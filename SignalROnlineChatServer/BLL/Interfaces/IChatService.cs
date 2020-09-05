using SignalROnlineChatServer.Models.ModelViews;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.BLL.Services.Interfaces
{
    public interface IChatService
    {
        Task<MessageViewModel> ReturnSendedMessageAsync(int groupId, string message);
        List<string> GetUserConnectionIdList(int chatId);
        Task IncreaseUsersUnreadMessageCount(int chatId);
        Task ReduceUserUnreadMessageCount(int chatId);
    }
}