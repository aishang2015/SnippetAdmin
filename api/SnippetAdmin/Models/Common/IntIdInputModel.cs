namespace SnippetAdmin.Models.Common
{
    public record IdInputModel<T>
    {
        public T Id { get; set; }
    }
}