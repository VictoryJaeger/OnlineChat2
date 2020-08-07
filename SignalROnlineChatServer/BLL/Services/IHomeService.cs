using SignalROnlineChatServer.Models;
using SignalROnlineChatServer.Models.ModelViews;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.BLL.Services
{
    public interface IHomeService
    {
        Chat CheckPrivateChat(string Id /*string ActiveUserId*/);
        void CreateGroupAsync(CreateGroupModelView groupModel, string CreatorId);
        void CreateMessageAsync(int groupId, string message);
        Task<Chat> CreatePrivateChatAsync(string ParticipantId/*, string CreatorId*/);
        IEnumerable<ChatViewModel> GetAllChats(/*string user*/);
        ChatViewModel GetChat(int id);
        User GetUser(string Id);
        List<UserViewModel> GetUsers(/*string ActiveUserId*/);
        MessageType CheckMessagesType(MessageViewModel model);
        Task<ChatViewModel> ReturnCreatedPrivateChatAsync(string ParticipantId);
    }
}