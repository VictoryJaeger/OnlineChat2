using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Models
{
    public class User: IdentityUser
    {
        public virtual ICollection<ChatUser> Chats { get; set; }
        public ICollection<Connection> Connections { get; set; }
    }
}
