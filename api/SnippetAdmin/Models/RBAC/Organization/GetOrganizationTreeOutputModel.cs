namespace SnippetAdmin.Models.RBAC.Organization
{
    public class GetOrganizationTreeOutputModel
    {
        public string Title { get; set; }

        public string Icon { get; set; }

        public int Key { get; set; }

        public List<GetOrganizationTreeOutputModel> Children { get; set; }
    }
}