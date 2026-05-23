using hydash.WebApp.Data;
using hydash.WebApp.Data.EF;
using hydash.WebApp.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hydash.WebApp.Components.Account
{
    public class RoleService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleService(IDbContextFactory<ApplicationDbContext> dbContextFactory, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _dbContextFactory = dbContextFactory;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // Create a role if it doesn't exist
        public async Task CreateRoleAsync(string roleName, string description, string colorCode, RolePriority priority)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var role = new ApplicationRole { Name = roleName, NormalizedName = roleName.ToUpper(), Description = description, ColorCode = colorCode, Priority = Convert.ToInt32(priority) };
                    await _roleManager.CreateAsync(role);
                }
            }
        }

        // Delete a role by name
        public async Task DeleteRoleAsync(string roleName)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    await _roleManager.DeleteAsync(role);
                }
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
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await _roleManager.RoleExistsAsync(roleName);
            }
        }

        // Get Roles from user
        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                return userRoles;
            }
        }

        // Get Role with highest priority from user
        public async Task<ApplicationRole> GetUserHighestPriorityRoleAsync(ApplicationUser user)
        {
            using (var context = _dbContextFactory.CreateDbContext())
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
        }

        // Get role
        public async Task<ApplicationRole> GetRoleAsync(string roleName)
        {
            var roleExists = await RoleExistsAsync(roleName);

            using (var context = _dbContextFactory.CreateDbContext())
            {
                ApplicationRole memberRole = await _roleManager.FindByNameAsync("Member");

                if (!roleExists)
                {
                    memberRole = await _roleManager.FindByNameAsync(roleName);
                }
                return memberRole;
            }
        }

        // Check if user has role else give member role back
        public async Task<ApplicationRole> GetUserRoleAsync(ApplicationUser user)
        {
            var highestUserRole = await GetUserHighestPriorityRoleAsync(user);

            if (highestUserRole != null)
            {
                return highestUserRole;
            }

            ApplicationRole memberRole = await GetRoleAsync("Member");
            return memberRole;
        }

        public async Task UpdateRoleAsync(ApplicationRole role)
        {
            await _roleManager.UpdateAsync(role);
        }

        public async Task<List<Permission>> GetAllPermissionsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Permissions.ToListAsync();
        }

        public async Task<List<int>> GetRolePermissionIdsAsync(string roleId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .ToListAsync();
        }

        public async Task SetRolePermissionsAsync(string roleId, IEnumerable<int> permissionIds)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var existing = context.RolePermissions.Where(rp => rp.RoleId == roleId);
            context.RolePermissions.RemoveRange(existing);
            foreach (var permId in permissionIds)
                context.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = permId });
            await context.SaveChangesAsync();
        }
    }
}