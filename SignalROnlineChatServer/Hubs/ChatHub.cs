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

        public override async Task OnConnectedAsync()
        {
            //await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            var userName = Context.User.Identity.Name;

            var user = _context.Users.Include(x => x.Connections).Where(x => x.UserName == userName).FirstOrDefault();

            var connection = new Connection {
                ConnectionID = GetConnectionId(),
                Connected = true
            };

            user.Connections.Add(connection);

            //user.Connections.Add(new Connection
            //{
            //    ConnectionID = GetConnectionId(),
            //    Connected = true
            //});

            await _context.SaveChangesAsync();

            await base.OnConnectedAsync();
        }
        //public string GetUserAgent() =>
        //    Context.Request.Headers["User-Agent"];
    }
}
