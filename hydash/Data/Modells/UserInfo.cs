using Microsoft.AspNetCore.Mvc;

public class UserInfo
{
	public bool IsAuthenticated { get; set; }
	public string Email { get; set; }
	public string Username { get; set; }
	public List<string> Roles { get; set; }
}
