﻿namespace SnippetAdmin.Endpoint.Models.RBAC.Role
{
	public record AddOrUpdateRoleInputModel
	{
		public int? Id { get; set; }

		public string Name { get; set; }

		public string Code { get; set; }

		public string Remark { get; set; }

		public int[] Rights { get; set; }
	}
}