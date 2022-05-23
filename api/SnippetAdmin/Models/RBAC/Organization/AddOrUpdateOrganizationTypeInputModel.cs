namespace SnippetAdmin.Models.RBAC.Organization
{
    public record AddOrUpdateOrganizationTypeInputModel
    {
        public int? Id { get; set; }

        public string Name { get; set; }
    }
}
