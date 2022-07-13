namespace SnippetAdmin.Endpoint.Models.RBAC.Organization
{
    public record GetOrganizationTypesOutputModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }
    }
}
