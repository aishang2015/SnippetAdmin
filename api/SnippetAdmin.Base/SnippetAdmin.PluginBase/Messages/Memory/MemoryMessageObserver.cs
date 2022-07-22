using System.Collections.Concurrent;
using System.Threading.Channels;

namespace SnippetAdmin.PluginBase.Messages.Memory
{
    public class MemoryMessageObserver : IMessageObserver
    {
        private readonly ConcurrentDictionary<string, Channel<string>> _topicChannelDic = new();

        private readonly ConcurrentDictionary<string, IList<IMessageHandler>> _topicHandlerDic = new();

        public MemoryMessageObserver()
        {
            new Task(async () =>
            {
                while (true)
                {
                    await Task.Delay(2000);
                    _topicChannelDic.AsParallel().ForAll(async t =>
                    {
                        var messageReader = t.Value.Reader;
                        var messageList = messageReader.ReadAllAsync();

                        await foreach (var message in messageList)
                        {
                            if (_topicHandlerDic.ContainsKey(t.Key))
                            {
                                var handlers = _topicHandlerDic[t.Key];
                                handlers.AsParallel().ForAll(async handler =>
                                {
                                    await handler.HandleAsync(message);
                                });
                            }
                        }
                    });
                }

            }, TaskCreationOptions.LongRunning).Start();
        }

        public void Publish(string topicName, string content)
        {
            if (!_topicChannelDic.ContainsKey(topicName))
            {
                _topicChannelDic.TryAdd(topicName, Channel.CreateUnbounded<string>());
            }
            var channel = _topicChannelDic[topicName];
            channel.Writer.TryWrite(content);
        }

        public void Subscribe(string topicName, IMessageHandler handler)
        {
            if (!_topicHandlerDic.ContainsKey(topicName))
            {
                _topicHandlerDic.TryAdd(topicName, new List<IMessageHandler> { handler });
            }
            else
            {
                var handlers = _topicHandlerDic[topicName];
                handlers.Add(handler);
            }
        }

        public void UnSubscribe(string topicName, IMessageHandler handler)
        {
            if (_topicHandlerDic.ContainsKey(topicName))
            {
                var handlers = _topicHandlerDic[topicName];
                var needRemovedHandler = handlers.FirstOrDefault(h => h.GetType() == handler.GetType());
                if (needRemovedHandler != null)
                {
                    handlers.Remove(needRemovedHandler);
                }
            }
        }
    }
}
