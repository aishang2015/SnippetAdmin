namespace SnippetAdmin.Models.Common
{
    public record IdInputModel<T> where T : struct
    {
        public T Id { get; set; }
    }
}