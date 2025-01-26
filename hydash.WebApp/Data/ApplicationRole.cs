using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace hydash.WebApp.Data
{
	// Add profile data for application roles by adding properties to the ApplicationRole class
	public class ApplicationRole : IdentityRole
	{
		public string Description { get; set; }
		public string ColorCode { get; set; }
		public int Priority { get; set; }
	}
}
