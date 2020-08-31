using AutoMapper;
using Microsoft.AspNetCore.Http;
using SignalROnlineChatServer.DataBase;
using SignalROnlineChatServer.Models;
using SignalROnlineChatServer.Models.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.BLL.Services
{
    public class ChatService : IChatService
    {
        private readonly OnlineChatDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public ChatService(OnlineChatDBContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<MessageViewModel> SendMessageAsync(int groupId, string message)
        {
            var newMessage = new Message
            {
                ChatId = groupId,
                Timestamp = DateTime.Now,
                Text = message,
                Name = _httpContextAccessor.HttpContext.User.Identity.Name
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            var messageView = _mapper.Map<MessageViewModel>(newMessage);

            return messageView;
        }
    }
}
