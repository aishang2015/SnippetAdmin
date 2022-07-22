namespace SnippetAdmin.PluginBase.Messages
{
    public interface IMessageObserver
    {
        public void Subscribe(string topicName, IMessageHandler handler);

        public void Publish(string topicName, string content);

        public void UnSubscribe(string topicName, IMessageHandler handler);
    }
}
