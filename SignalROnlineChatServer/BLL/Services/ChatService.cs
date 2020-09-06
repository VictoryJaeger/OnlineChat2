using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SignalROnlineChatServer.BLL.Services.Interfaces;
using SignalROnlineChatServer.DataBase;
using SignalROnlineChatServer.Models;
using SignalROnlineChatServer.Models.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.BLL.Services
{
    public class ChatService : IChatService
    {
        private readonly OnlineChatDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IHomeService _homeService;
        public ChatService(OnlineChatDBContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper, IHomeService homeService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _homeService = homeService;
        }

        public async Task<MessageViewModel> ReturnSendedMessageAsync(int groupId, string message)
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

        public List<string> GetUserConnectionIdList(int chatId)
        {
            var chat = _context.Chats
                .Include(x => x.ChatParticipants).ThenInclude(x => x.User).ThenInclude(x => x.Connections)
                .Where(x => x.Id == chatId)
                .FirstOrDefault();

            var connectionIdList = _context.Users
               .Include(x => x.Connections)
               .Where(x => chat.ChatParticipants
                       .Select(u => u.UserId).ToList()
                       .Contains(x.Id))
                .AsNoTracking()
                .AsEnumerable()
                .Select(c => c.Connections.Last().ConnectionID).ToList();

            return connectionIdList;
        }

        public async Task IncreaseUsersUnreadMessageCount(int chatId)
        {
            var chat = _homeService.GetChat(chatId);
            var actionUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var chatUsers = chat.ChatParticipants.Where(x => x.UserId != actionUserId).ToList();

            foreach(var user in chatUsers)
            {
                user.UnreadMessageCount++;
            }

            await _context.SaveChangesAsync();
        }

        public async Task ReduceUserUnreadMessageCount(int chatId)
        {
            var chat = _homeService.GetChat(chatId);
            var actionUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = chat.ChatParticipants.Where(x => x.UserId == actionUserId).FirstOrDefault();
            user.UnreadMessageCount = 0;

            await _context.SaveChangesAsync();
        }
    }
}
