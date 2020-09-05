//using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalROnlineChatServer.DataBase;
using SignalROnlineChatServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Hubs
{
    public class ChatHub: Hub
    {
        private readonly OnlineChatDBContext _context;

        public ChatHub(OnlineChatDBContext context)
        {
            _context = context;
        }
        public string GetConnectionId() =>
          Context.ConnectionId;
    }
}
