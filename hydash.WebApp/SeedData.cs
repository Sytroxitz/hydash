using hydash.WebApp.Data;
using hydash.WebApp.Data.EF;
using Microsoft.AspNetCore.Identity;
using hydash.WebApp.Components.Account;
using hydash.WebApp.Data.Enums;
using Microsoft.EntityFrameworkCore;

internal class Role
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Color { get; set; }
    public RolePriority Priority { get; set; }
}

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleService roleManager, IConfiguration configuration)
	{
		// Roles to seed
		var roles = new Dictionary<string, Role>
		{
			["admin"] = new Role { Name = "Administrator", Description = "Administrator Role", Color = "#FF0000", Priority = RolePriority.SUPERUSER },
			["member"] = new Role { Name = "Member", Description = "Member Role", Color = "#FFFFFF", Priority = RolePriority.MEMBER },
		};

        // Create roles if they do not exist
        foreach (var (key, role) in roles)
        {
			await roleManager.CreateRoleAsync(role.Name, role.Description, role.Color, role.Priority);
        }

		// Seed Permission records from the Permissions enum
		var dbFactory = serviceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
		using (var context = dbFactory.CreateDbContext())
		{
			foreach (Permissions perm in Enum.GetValues<Permissions>())
			{
				var permName = perm.ToString();
				if (!await context.Permissions.AnyAsync(p => p.Name == permName))
					context.Permissions.Add(new Permission { Name = permName, Description = permName });
			}
			await context.SaveChangesAsync();
		}

		// Seed an Admin user
		var adminEmail = configuration["hydash:DefaultAdminEmail"];
		var adminUser = await userManager.FindByEmailAsync(adminEmail);

		if (adminUser != null)
		{
			await userManager.AddToRoleAsync(adminUser, "Administrator");
		}
	}
}
