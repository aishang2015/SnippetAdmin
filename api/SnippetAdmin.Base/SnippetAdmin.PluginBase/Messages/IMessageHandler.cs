namespace SnippetAdmin.PluginBase.Messages
{
    public interface IMessageHandler
    {
        public Task HandleAsync(string message);
    }
}
