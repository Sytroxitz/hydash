using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MudBlazor;

public class HydashAuthStateProvider : AuthenticationStateProvider
{
	private readonly SignInManager<IdentityUser> SignInManager;
	private readonly IConfiguration Configuration;

	public HydashAuthStateProvider(SignInManager<IdentityUser> signInManager, IConfiguration configuration)
	{
		SignInManager = signInManager;
		Configuration = configuration;
	}
	public void NotifyUserAuthentication(string email)
	{
		var identity = new ClaimsIdentity(new[]
		{
			new Claim(ClaimTypes.Email, email)
		}, "apiauth");
		var user = new ClaimsPrincipal(identity);
		NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
	}

	public void NotifyUserLogout()
	{
		var identity = new ClaimsIdentity();
		var user = new ClaimsPrincipal(identity);
		NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		HttpClient Http = new HttpClient();
		var userInfo = await Http.GetFromJsonAsync<UserInfo>(Configuration["Default:Uri"] + "api/v1/accounts/auto-login");
		if (userInfo.IsAuthenticated)
		{
			var identity = new ClaimsIdentity(new[]
			{
				new Claim(ClaimTypes.Email, userInfo.Email),
			}, "custom");

			return new AuthenticationState(new ClaimsPrincipal(identity));
		}
		else
		{
			var identity = new ClaimsIdentity();
			var user = new ClaimsPrincipal(identity);
			return new AuthenticationState(user);
		}
	}
}