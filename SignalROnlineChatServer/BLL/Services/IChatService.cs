using SignalROnlineChatServer.Models.ModelViews;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.BLL.Services
{
    public interface IChatService
    {
        Task<MessageViewModel> SendMessageAsync(int groupId, string message);
    }
}