namespace SnippetAdmin.Endpoint.Models.Common
{
    public record PagedOutputModel<T>
    {
        public int Total { get; set; }

        public IEnumerable<T> Data { get; set; }
    }
}