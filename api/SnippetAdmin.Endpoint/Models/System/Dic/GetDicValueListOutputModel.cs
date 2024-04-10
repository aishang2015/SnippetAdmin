namespace SnippetAdmin.Endpoint.Models.System.Dic
{
    public class GetDicValueListOutputModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Code { get; set; }

        public bool IsEnabled { get; set; }

        public int Sorting { get; set; }
    }
}
