namespace SnippetAdmin.Models.RBAC.Organization
{
    public class GetOrganizationOutputModel
    {
        public int Id { get; set; }

        public int? UpId { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string[] UpPositions { get; set; }

        public PositionInfo[] Positions { get; set; }
    }

    public class PositionInfo
    {
        public string Name { get; set; }
        public bool VisibleToChild { get; set; }
    }
}