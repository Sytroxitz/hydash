using System.ComponentModel.DataAnnotations;

namespace hydash.WebApp.Data.EF
{
	public class Permission
	{
		public int Id { get; set; }

		[MaxLength(255)] // Specify length to avoid index issues
		public string Name { get; set; }

		public string Description { get; set; }
	}
}
