namespace SnippetAdmin.Endpoint.Models.RBAC.User
{
	public record AddOrUpdateUserInputModel
	{
		public int? Id { get; set; }

		public string? UserName { get; set; }

		public string? RealName { get; set; }

		public int Gender { get; set; }

		public string? PhoneNumber { get; set; }

		public int[]? Roles { get; set; }

		public int[]? Organizations { get; set; }

		public int[]? Positions { get; set; }
	}
}