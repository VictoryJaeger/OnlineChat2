﻿using SignalROnlineChatServer.Models.ModelViews;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.BLL.Services
{
    public interface IChatService
    {
        Task<MessageViewModel> SendMessageAsync(int groupId, string message);
        List<string> GetUserConnectionIdList(int chatId, string connectionId);
    }
}