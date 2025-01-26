namespace hydash.WebApp.Data.EF
{
	public class RolePermission
	{
		public int RolePermissionId { get; set; }

		public string RoleId { get; set; }

		public ApplicationRole Role { get; set; }


		public int PermissionId { get; set; }

		public Permission Permission { get; set; }
	}
}
