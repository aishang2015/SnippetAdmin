namespace SnippetAdmin.Models.RBAC.Element
{
    public class GetElementOutputModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Type { get; set; }

        public string Identity { get; set; }

        public string AccessApi { get; set; }
    }
}