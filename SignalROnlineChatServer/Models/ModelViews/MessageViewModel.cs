using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Models.ModelViews
{
    public class MessageViewModel
    {
        public int ChatId { get; set; }
        public string Text { get; set; }
    }
}
