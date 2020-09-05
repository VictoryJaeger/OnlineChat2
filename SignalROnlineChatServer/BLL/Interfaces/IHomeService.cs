using SignalROnlineChatServer.Models;
using SignalROnlineChatServer.Models.ModelViews;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.BLL.Services.Interfaces
{
    public interface IHomeService
    {
        Chat CheckPrivateChat(string Id);
        Task<Chat> CreateGroupAsync(CreateGroupModelView groupModel);
        Task<Chat> CreatePrivateChatAsync(string ParticipantId);
        IEnumerable<ChatViewModel> GetAllChats();
        ChatViewModel GetChatView(Chat chat);
        Chat GetChat(int id);
        User GetUser(string Id);
        List<UserViewModel> GetUsers();
        Task<ChatViewModel> ReturnCreatedPrivateChatAsync(string ParticipantId);
        Task<ChatViewModel>ReturnCreatedGroupAsync(CreateGroupModelView groupModel);
        ChatViewModel GetPrivateChat(string Id);
        List<string> GetUserConnectionIdList(int chatId, string connectionId);
        Task DeleteChat(int id);
        string GetActiveUserName();
    }
}