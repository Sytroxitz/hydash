using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using hydash.Server;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/v1/[controller]")]
public class AuthenticationController : ControllerBase
{
	private readonly IConfiguration Configuration;

	public AuthenticationController(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	[HttpPost]
	[Route("login")]
	public IActionResult Login([FromBody] UserCredentials credentials)
	{
		if (IsValidUser(credentials))
		{
			var token = GenerateJwtToken();
			return Ok(token);
		}

		return Unauthorized();
	}

	private string GenerateJwtToken()
	{
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(Configuration["Jwt:Issuer"],
			Configuration["Jwt:Audience"],
			//expires: DateTime.Now.AddMinutes(120),
			expires: DateTime.Now.AddSeconds(10),
			signingCredentials: credentials);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	private bool IsValidUser(UserCredentials credentials)
	{
		// This is a placeholder. Replace it with actual validation logic.
		// For example, check credentials.Username and credentials.Password against a database.
		return credentials.Username == "admin" && credentials.Password == "password";
	}
}
public class UserCredentials
{
	public string Username { get; set; }
	public string Password { get; set; }
}