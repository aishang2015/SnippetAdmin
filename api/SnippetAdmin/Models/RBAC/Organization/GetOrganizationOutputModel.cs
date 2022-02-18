namespace SnippetAdmin.Models.RBAC.Organization
{
    public class GetOrganizationOutputModel
    {
        public int Id { get; set; }

        public int? UpId { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string Type { get; set; }

        public string TypeName { get; set; }

        public string Icon { get; set; }

        public string IconId { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }
    }
}