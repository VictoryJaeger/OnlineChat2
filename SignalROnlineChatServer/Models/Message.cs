using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public string Name { get; set; }

        public int ChatId { get; set; }
        [ForeignKey("ChatId")]
        public Chat Chat { get; set; }


    }
}
