namespace SnippetAdmin.Endpoint.Models.RBAC.User
{
	public record SearchUserOutputModel
	{
		public int Id { get; set; }

		public string Avatar { get; set; }

		public string UserName { get; set; }

		public string RealName { get; set; }

		public int Gender { get; set; }

		public string PhoneNumber { get; set; }

		public bool IsActive { get; set; }

		public RoleInfo[] Roles { get; set; }

		public string[] Organizations { get; set; }

		public string[] Positions { get; set; }
	}

	public class RoleInfo
	{
		public string RoleName { get; set; }

		public bool IsActive { get; set; }
	}
}