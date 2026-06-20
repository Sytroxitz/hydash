using hydash.WebApp.Data;
using hydash.WebApp.Data.EF;
using hydash.WebApp.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hydash.WebApp.Components.Account
{
    public class RoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _contextFactory = contextFactory;
        }

        // Create a role if it doesn't exist
        public async Task CreateRoleAsync(string roleName, string description, string colorCode, RolePriority priority)
        {
            using (var context = _contextFactory.CreateDbContext())
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
            using (var context = _contextFactory.CreateDbContext())
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
            using (var context = _contextFactory.CreateDbContext())
            {
                return await _roleManager.RoleExistsAsync(roleName);
            }
        }

        // Get Roles from user
        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                return userRoles;
            }
        }

        // Get Role with highest priority from user
        public async Task<ApplicationRole> GetUserHighestPriorityRoleAsync(ApplicationUser user)
        {
            using (var context = _contextFactory.CreateDbContext())
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

            await using (var context = await _contextFactory.CreateDbContextAsync())
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

        // Update an existing role
        public async Task UpdateRoleAsync(ApplicationRole role)
        {
            await _roleManager.UpdateAsync(role);
        }

        public async Task<List<Permission>> GetAllPermissionsAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Permissions.ToListAsync();
        }

        // Get the permission ids currently assigned to a role
        public async Task<List<int>> GetRolePermissionIdsAsync(string roleId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .ToListAsync();
        }

        // Replace the set of permissions assigned to a role
        public async Task SetRolePermissionsAsync(string roleId, IEnumerable<int> permissionIds)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var existing = await context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();
            context.RolePermissions.RemoveRange(existing);
            
            foreach (var permissionId in permissionIds.Distinct())
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                });
            }
            await context.SaveChangesAsync();
        }
    }
}