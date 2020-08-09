﻿//using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Hubs
{
    public class ChatHub: Hub
    {
        public string GetConnectionId() =>
          Context.ConnectionId;

        //public string GetUserAgent() =>
        //    Context.Request.Headers["User-Agent"];
    }
}
