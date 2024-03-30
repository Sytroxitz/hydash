using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class testController : ControllerBase
{
	[HttpGet]
	public ActionResult<string> Get()
	{
		return "Hello, Swagger!";
	}
}