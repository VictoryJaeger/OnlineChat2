using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Models.ModelViews
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Timestamp { get; set; }
        public string Name { get; set; }
        public MessageType Type { get; set; }

        //public int ChatId { get; set; }
        //public string Text { get; set; }
    }

    public enum MessageType
    {
        Incoming,
        Outgoing
    }
}
