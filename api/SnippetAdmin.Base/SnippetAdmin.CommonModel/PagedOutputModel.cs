namespace SnippetAdmin.CommonModel
{
    public record PagedOutputModel<T>
    {
        public int Total { get; set; }

        public IEnumerable<T> Data { get; set; }
    }
}