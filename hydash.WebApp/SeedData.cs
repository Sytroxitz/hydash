using hydash.WebApp.Data;
using Microsoft.AspNetCore.Identity;
using hydash.WebApp.Components.Admin;
using hydash.WebApp.Components.Account;
using hydash.WebApp.Data.Enums;

public static class SeedData
{
	public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleService roleManager, IConfiguration configuration)
	{
		// Roles to seed
		string[] roleNames = { "Administrator" };

		// Create roles if they do not exist
		foreach (var roleName in roleNames)
		{
			await roleManager.CreateRoleAsync(roleName, "Administrator Role", "#FF0000", RolePriority.SUPERUSER);
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
