using SignalROnlineChatServer.Models;
using SignalROnlineChatServer.Models.ModelViews;
using System.Collections.Generic;

namespace SignalROnlineChatServer.BLL.Services
{
    public interface IHomeService
    {
        bool CheckPrivateChat(string Id, string ActiveUserId);
        void CreateGroupAsync(CreateGroupModelView groupModel, string CreatorId);
        void CreateMessageAsync(int groupId, string message);
        void CreatePrivateChatAsync(string ParticipantId, string CreatorId);
        IEnumerable<ChatViewModel> GetAllChats(User user);
        ChatViewModel GetChat(int id);
        User GetUser(string Id);
        IEnumerable<UserViewModel> GetUsers(string ActiveUserId);
    }
}