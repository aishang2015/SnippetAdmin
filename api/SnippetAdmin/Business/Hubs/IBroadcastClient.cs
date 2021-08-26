using System.Threading.Tasks;

namespace SnippetAdmin.Business.Hubs
{
    /// <summary>
    /// 定义客户端singalr方法
    /// </summary>
    public interface IBroadcastClient
    {
        /// <summary>
        /// 客户端接收消息方法
        /// </summary>
        Task HandleMessage(string message);
    }
}