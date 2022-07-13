namespace SnippetAdmin.Endpoint.Models.RBAC.Organization
{
    public record UpdateOrganizationInputModel
    {
        public int? UpId { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string Type { get; set; }

        public string Icon { get; set; }

        public string IconId { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public int Sorting { get; set; }
    }
}