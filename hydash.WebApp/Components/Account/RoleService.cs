using hydash.WebApp.Data;
using hydash.WebApp.Data.Enums;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hydash.WebApp.Components.Account
{
    public class RoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // Create a role if it doesn't exist
        public async Task CreateRoleAsync(string roleName, string description, string colorCode, RolePriority priority)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var role = new ApplicationRole { Name = roleName, NormalizedName = roleName.ToUpper(), Description = description, ColorCode = colorCode, Priority = Convert.ToInt32(priority) };
                await _roleManager.CreateAsync(role);
            }
        }

        // Delete a role by name
        public async Task DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
        }

        // Get all roles
        public IEnumerable<ApplicationRole> GetAllRoles()
        {
            return _roleManager.Roles;
        }

        // Check if a role exists
        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }

        // Get Roles from user
        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        // Get Role with highest priority from user
        public async Task<ApplicationRole> GetUserHighestPriorityRoleAsync(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            ApplicationRole highestPriorityRole = null;

            foreach (var roleName in userRoles)
            {
                // Get the role from the RoleManager
                var role = await _roleManager.FindByNameAsync(roleName);

                if (role != null)
                {
                    // Update if this role has a higher priority
                    if (highestPriorityRole == null || role.Priority > highestPriorityRole.Priority)
                    {
                        highestPriorityRole = role;
                    }
                }
            }

            return highestPriorityRole;
        }

        // Add other role-related methods as needed
    }
}