using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Models
{
    public class Chat
    {

        public Chat()
        {
            Messages = new List<Message>();
            ChatParticipants = new List<ChatUser>();
        }
        public int Id { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<ChatUser> ChatParticipants { get; set; }
        public ChatType Type { get; set; } 
        public string Name { get; set; }
        //public int UnreadMessages { get; set; }
    }

    public enum ChatType
    {
        Private,
        Group
    }
}
