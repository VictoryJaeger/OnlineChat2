using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Models
{
    public class ChatUser
    {        
        public string UserId { get; set; }
        public User User { get; set; }
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
        public UserRole Role { get; set; }
        public int UnreadMessageCount { get; set; }

    }   

    public enum UserRole
    {
        Admin,
        Member,
        Guest
    }
}

