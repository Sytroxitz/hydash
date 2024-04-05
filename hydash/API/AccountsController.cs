using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
public class AccountsController : ControllerBase
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly SignInManager<IdentityUser> _signInManager;

	public AccountsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] RegisterModel model)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		var user = new IdentityUser { UserName = model.Username, Email = model.Email};
		var result = await _userManager.CreateAsync(user, model.Password);

		if (result.Succeeded)
		{
			// Here you could also sign the user in after registration
			// await _signInManager.SignInAsync(user, isPersistent: false);
			return Ok(new { message = "User registered successfully!" });
		}

		foreach (var error in result.Errors)
		{
			ModelState.AddModelError("", error.Description);
		}

		return BadRequest(ModelState);
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginModel model)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		var user = await _userManager.FindByEmailAsync(model.Email);
		var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

		if (result.Succeeded)
		{
			return Ok(new { message = "Login successful!" });
		}
		else if (result.IsLockedOut)
		{
			return BadRequest(new { message = "User account locked out." });
		}
		else
		{
			ModelState.AddModelError(string.Empty, "Invalid login attempt.");
			return BadRequest(ModelState);
		}
	}

	[HttpGet("auto-login")]
	public IActionResult AutoLogin()
	{
		var user = new UserInfo();
		if (User.Identity.IsAuthenticated)
		{
			return Ok(new { IsAuthenticated = true, user.Username, user.Email });
		}
		else
		{
			return Ok(new { IsAuthenticated = false });
		}
	}
}