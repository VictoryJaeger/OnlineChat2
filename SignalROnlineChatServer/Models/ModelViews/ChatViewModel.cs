using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Models.ModelViews
{
    public class ChatViewModel
    {
        public ChatViewModel()
        {

        }
        
        public int Id { get; set; }
        public  List<MessageViewModel> Messages { get; set; }
        public  List<UserViewModel> ChatParticipants { get; set; }
        public string Name { get; set; }
        public string LastMessageDate { get; set; }
        public string LastMessage { get; set; }
        public string LastMessageAuthor { get; set; }
        public List<string> UsersConnectionId { get; set; }
        public int UnreadMessages { get; set; }

    }
    
}






//public ChatViewModel(int Id, ICollection<Message> Messages, ICollection<ChatUser> ChatParticipants, string Name)
//{
//    this.Id = Id;
//    this.Name = Name;
//    this.Messages = Messages.ToList();
//    this.ChatParticipants = ChatParticipants.ToList();
//    LastMessage = this.Messages.Last().Text;
//    LastMessageDate = this.Messages.Last().Timestamp.Date.ToString();

//}


//public MessageType MessageType { get; set; }

//public void PrepareModelForView(ChatViewModel model)
//{
//    model.LastMessage = model.Messages.Last().Text;
//    model.LastMessageDate = model.Messages.Last().Timestamp.Date.ToString();
//    foreach(var message in model.Messages)
//    {
//        if(message.Name == )
//    }
//}

//public enum MessageType
//{
//    Incoming,
//    Outcoming
//}