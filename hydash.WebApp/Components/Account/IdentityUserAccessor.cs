using hydash.WebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace hydash.WebApp.Components.Account
{
	internal sealed class IdentityUserAccessor
	{
        private readonly IServiceScopeFactory _scopeFactory;
		private IdentityRedirectManager _redirectManager;

		public IdentityUserAccessor(IServiceScopeFactory scopeFactory, IdentityRedirectManager redirectManager)
		{
			_scopeFactory = scopeFactory;
			_redirectManager = redirectManager;
		}

		public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
		{
			await using var scope = _scopeFactory.CreateAsyncScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.GetUserAsync(context.User);

			if (user is null)
			{
				_redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
			}

			return user;
		}
	}
}
