using SignalROnlineChatServer.Models;
using SignalROnlineChatServer.Models.ModelViews;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.BLL.Services
{
    public interface IHomeService
    {
        Chat CheckPrivateChat(string Id /*string ActiveUserId*/);
        Task<Chat> CreateGroupAsync(CreateGroupModelView groupModel);
        void CreateMessageAsync(int groupId, string message);
        Task<Chat> CreatePrivateChatAsync(string ParticipantId/*, string CreatorId*/);
        IEnumerable<ChatViewModel> GetAllChats(/*string user*/);
        ChatViewModel GetChat(int id);
        User GetUser(string Id);
        List<UserViewModel> GetUsers(/*string ActiveUserId*/);
        //MessageType CheckMessagesType(MessageViewModel model);
        Task<ChatViewModel> ReturnCreatedPrivateChatAsync(string ParticipantId);
        Task<ChatViewModel>ReturnCreatedGroupAsync(CreateGroupModelView groupModel);
        ChatViewModel GetPrivateChat(string Id);
        List<string> GetUserConnectionIdList(int chatId, string connectionId);
    }
}