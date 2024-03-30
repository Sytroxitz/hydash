using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class SecureController : ControllerBase
{
	[HttpGet]
	public IActionResult Get()
	{
		// Only accessible if the user has a valid JWT
		return Ok("You are authenticated");
	}
}