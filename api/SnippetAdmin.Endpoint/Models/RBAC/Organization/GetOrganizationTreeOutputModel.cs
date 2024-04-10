namespace SnippetAdmin.Endpoint.Models.RBAC.Organization
{
    public record GetOrganizationTreeOutputModel
    {
        public string? Title { get; set; }

        public int Value { get; set; }

        public string? Icon { get; set; }

        public string? IconId { get; set; }

        public int Key { get; set; }

        public List<GetOrganizationTreeOutputModel> Children { get; set; } = new();
    }
}