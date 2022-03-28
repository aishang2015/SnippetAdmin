namespace SnippetAdmin.Models.Dynamic
{
    public class GetDynamicInfoOutputModel
    {
        public string Group { get; set; }

        public List<DynamicInfoGroup> DynamicInfoGroups { get; set; }
    }

    public class DynamicInfoGroup
    {
        public string Name { get; set; }

        public string EntityName { get; set; }
    }
}
