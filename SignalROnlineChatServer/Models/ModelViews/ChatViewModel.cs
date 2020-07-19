using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Models.ModelViews
{
    public class ChatViewModel
    {
        public ChatViewModel(int Id, ICollection<Message> Messages, ICollection<ChatUser> ChatParticipants, string Name)
        {
            this.Id = Id;
            this.Name = Name;
            this.Messages = Messages.ToList();
            this.ChatParticipants = ChatParticipants.ToList();

        }
        public int Id { get; set; }
        public  List<Message> Messages { get; set; }
        public  List<ChatUser> ChatParticipants { get; set; }
        public string Name { get; set; }
    }
}
