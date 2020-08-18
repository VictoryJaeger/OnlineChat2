using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalROnlineChatServer.DataBase;
using SignalROnlineChatServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Hubs
{
    public class HomeHub: Hub
    {
        private readonly OnlineChatDBContext _context;

        public HomeHub(OnlineChatDBContext context)
        {
            _context = context;
        }
        public string GetConnectionId() =>
          Context.ConnectionId;

        public override async Task OnConnectedAsync()
        {
            var userName = Context.User.Identity.Name;

            var user = _context.Users.Include(x => x.Connections).Where(x => x.UserName == userName).FirstOrDefault();

            var connection = new Connection
            {
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
    }
}
