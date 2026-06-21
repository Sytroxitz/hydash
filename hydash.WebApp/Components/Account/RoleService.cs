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

        // The Superuser role is identified by its priority, not its name.
        // Only a single role at this priority may ever exist, and it cannot be deleted or demoted.
        private const int SuperuserPriority = (int)RolePriority.SUPERUSER;

        // Whether a role with Superuser priority already exists.
        public async Task<bool> SuperuserRoleExistsAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Roles.AnyAsync(r => r.Priority == SuperuserPriority);
        }

        // Create a role if it doesn't exist
        public async Task<(bool Success, string? Error)> CreateRoleAsync(string roleName, string description, string colorCode, RolePriority priority)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return (false, $"A role named '{roleName}' already exists.");
            }

            if (priority == RolePriority.SUPERUSER && await SuperuserRoleExistsAsync())
            {
                return (false, "A Superuser role already exists. Only one Superuser role may exist.");
            }

            var role = new ApplicationRole { Name = roleName, NormalizedName = roleName.ToUpper(), Description = description, ColorCode = colorCode, Priority = Convert.ToInt32(priority) };
            var result = await _roleManager.CreateAsync(role);
            return (result.Succeeded, result.Succeeded ? null : string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        // Delete a role by name
        public async Task<(bool Success, string? Error)> DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return (false, $"Role '{roleName}' not found.");
            }

            if (role.Priority == SuperuserPriority)
            {
                return (false, "The Superuser role cannot be deleted.");
            }

            var result = await _roleManager.DeleteAsync(role);
            return (result.Succeeded, result.Succeeded ? null : string.Join("; ", result.Errors.Select(e => e.Description)));
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
        public async Task<(bool Success, string? Error)> UpdateRoleAsync(ApplicationRole role)
        {
            ApplicationRole? original;
            await using (var context = await _contextFactory.CreateDbContextAsync())
            {
                original = await context.Roles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == role.Id);
            }

            bool wasSuperuser = original?.Priority == SuperuserPriority;
            bool willBeSuperuser = role.Priority == SuperuserPriority;

            // Promoting another role to Superuser while one already exists would create a second.
            if (willBeSuperuser && !wasSuperuser && await SuperuserRoleExistsAsync())
            {
                return (false, "A Superuser role already exists. Only one Superuser role may exist.");
            }

            // Demoting the sole Superuser would leave the system without one.
            if (wasSuperuser && !willBeSuperuser)
            {
                return (false, "The Superuser role cannot be demoted.");
            }

            // The Superuser role's name is referenced by [Authorize(Roles = "Administrator")];
            // renaming it would lock everyone out of the admin area.
            if (wasSuperuser && !string.Equals(original!.Name, role.Name, StringComparison.Ordinal))
            {
                return (false, "The Superuser role cannot be renamed.");
            }

            var result = await _roleManager.UpdateAsync(role);
            return (result.Succeeded, result.Succeeded ? null : string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        public async Task<List<Permission>> GetAllPermissionsAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Permissions.ToListAsync();
        }

        // Create a permission if no permission with the same name exists
        public async Task<(bool Success, string? Error)> CreatePermissionAsync(string name, string description)
        {
            name = name.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                return (false, "Permission name is required.");
            }

            await using var context = await _contextFactory.CreateDbContextAsync();
            if (await context.Permissions.AnyAsync(p => p.Name == name))
            {
                return (false, $"A permission named '{name}' already exists.");
            }

            context.Permissions.Add(new Permission { Name = name, Description = description });
            await context.SaveChangesAsync();
            return (true, null);
        }

        // Update an existing permission's name and description
        public async Task<(bool Success, string? Error)> UpdatePermissionAsync(int id, string name, string description)
        {
            name = name.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                return (false, "Permission name is required.");
            }

            await using var context = await _contextFactory.CreateDbContextAsync();
            var permission = await context.Permissions.FirstOrDefaultAsync(p => p.Id == id);
            if (permission == null)
            {
                return (false, "Permission not found.");
            }

            if (await context.Permissions.AnyAsync(p => p.Name == name && p.Id != id))
            {
                return (false, $"A permission named '{name}' already exists.");
            }

            permission.Name = name;
            permission.Description = description;
            await context.SaveChangesAsync();
            return (true, null);
        }

        // Delete a permission, first removing any role assignments referencing it
        public async Task<(bool Success, string? Error)> DeletePermissionAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var permission = await context.Permissions.FirstOrDefaultAsync(p => p.Id == id);
            if (permission == null)
            {
                return (false, "Permission not found.");
            }

            var assignments = await context.RolePermissions
                .Where(rp => rp.PermissionId == id)
                .ToListAsync();
            context.RolePermissions.RemoveRange(assignments);
            context.Permissions.Remove(permission);
            await context.SaveChangesAsync();
            return (true, null);
        }

        // Map of permission id -> number of (non-Superuser) roles that have it explicitly
        // assigned. The Superuser role implicitly holds every permission and is counted
        // separately by callers via SuperuserRoleExistsAsync.
        public async Task<Dictionary<int, int>> GetPermissionUsageCountsAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.RolePermissions
                .Where(rp => rp.Role.Priority != SuperuserPriority)
                .GroupBy(rp => rp.PermissionId)
                .Select(g => new { PermissionId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PermissionId, x => x.Count);
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

            // The Superuser role implicitly holds every permission (including future
            // ones); its permission set is not editable and cannot be reduced.
            var roleIsSuperuser = await context.Roles
                .AnyAsync(r => r.Id == roleId && r.Priority == SuperuserPriority);
            if (roleIsSuperuser)
            {
                return;
            }

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