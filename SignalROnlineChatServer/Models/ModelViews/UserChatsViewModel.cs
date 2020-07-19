using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Models.ModelViews
{
    public class UserChatsViewModel
    {
        public UserChatsViewModel(ICollection<ChatViewModel> chats)
        {
            Chats = chats.ToList();
        }
        public List<ChatViewModel> Chats { get; set; }
    }
}
