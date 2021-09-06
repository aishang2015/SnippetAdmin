namespace SnippetAdmin.Models.RBAC.Organization
{
    public class UpdateOrganizationInputModel
    {
        public int? UpId { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }
    }
}