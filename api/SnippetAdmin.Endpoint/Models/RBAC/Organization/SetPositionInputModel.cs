namespace SnippetAdmin.Endpoint.Models.RBAC.Organization
{
    public record SetPositionInputModel
    {
        public int OrganizationId { get; set; }

        public PositionModel[] Positions { get; set; }
    }

    public record PositionModel
    {
        public bool VisibleToChild { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }
    }
}