﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SignalROnlineChatServer.BLL.Services.Interfaces;
using SignalROnlineChatServer.DataBase;
using SignalROnlineChatServer.Models;
using SignalROnlineChatServer.Models.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.BLL.Services
{
    public class HomeService : IHomeService
    {
        private readonly OnlineChatDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public HomeService(OnlineChatDBContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public IEnumerable<ChatViewModel> GetAllChats()
        {
            var activeUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var chats = _context.Chats
                .Include(x => x.ChatParticipants).ThenInclude(x => x.User)
                .Include(x => x.Messages)
                .Where(x => x.ChatParticipants.Any(y => y.UserId == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value))
                .ToList();

            chats = chats.OrderByDescending(x => x?.Messages?.LastOrDefault()?.Timestamp?? DateTime.MinValue).ToList();

            var myChats = _mapper.Map<List<ChatViewModel>>(chats);

            return myChats;
        }   
        
        public Chat GetChat(int id)
        {
            var chat = _context.Chats
                .Include(x => x.Messages)
                .Include(y => y.ChatParticipants).ThenInclude(y => y.User).ThenInclude(y => y.Connections)
                .FirstOrDefault(x => x.Id == id);

            return chat;
        }

        public async Task DeleteChat(int id)
        {
            var chat = _context.Chats.Where(x => x.Id == id).FirstOrDefault();
            _context.Chats.Remove(chat);

            await _context.SaveChangesAsync();
        }

        public ChatViewModel GetChatView(Chat chat)
        {
            return _mapper.Map<ChatViewModel>(chat);
        }

        public Chat CheckPrivateChat(string Id)
        {
            var chat = _context.Chats
                .Include(x => x.ChatParticipants).ThenInclude(x => x.User)
                .Where(x => x.Type == ChatType.Private &&
                 x.ChatParticipants.Any(y => y.UserId == _context.Users.Where(x => 
                        x.Id == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value).FirstOrDefault().Id
                        && x.ChatParticipants.Any(y => y.UserId == _context.Users.Where(x => x.Id == Id).FirstOrDefault().Id)))
                .FirstOrDefault();

            if (chat == null) return null;

            return chat;

        }

        public async Task<Chat> CreatePrivateChatAsync(string ParticipantId)
        {

            var chat = new Chat
            {
                Type = ChatType.Private,
                Name = _context.Users.Where(x => x.Id == ParticipantId).FirstOrDefault().UserName + ',' +
                       _context.Users.Where(x => x.Id == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value).FirstOrDefault().UserName
            };

            chat.Messages.Add(new Message
            {
                Text = $"Chat \"{chat.Name}\" is created",
                Timestamp = DateTime.Now,
                Name = "Default"
            });

            chat.ChatParticipants.Add(new ChatUser
            {
                UserId = ParticipantId
            });

            chat.ChatParticipants.Add(new ChatUser
            {
                UserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value
            });

            _context.Chats.Add(chat);

            await _context.SaveChangesAsync();

            return chat;

        }

        public async Task<Chat> CreateGroupAsync(CreateGroupModelView groupModel)
        {
            var chat = new Chat
            {
                Name = groupModel.Name,                
                Type = ChatType.Group
            };


            foreach (var Id in groupModel.ChatParticipantsId.ToList())
            {
                chat.ChatParticipants.Add(new ChatUser
                {
                    UserId = _context.Users
                        .Where(x => x.Id == Id).FirstOrDefault().Id,
                    Role = UserRole.Member
                });
            }

            chat.ChatParticipants.Add(new ChatUser
            {
                UserId = _context.Users.Where(y => y.Id == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value).FirstOrDefault().Id
            });

            chat.Messages.Add(new Message
            {
                Text = $"Chat \"{chat.Name}\" is created",
                Timestamp = DateTime.Now,
                Name = "Default"
            });

            _context.Chats.Add(chat);

            await _context.SaveChangesAsync();

            return chat;
        }

        public async Task<ChatViewModel> ReturnCreatedPrivateChatAsync(string ParticipantId)
        {
            var chat = await CreatePrivateChatAsync(ParticipantId);

            var chatModel = _mapper.Map<ChatViewModel>(chat);

            var adminConnectionId = _context.Users
                .Include(x => x.Connections)
                .Where(x => x.Id == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)
                .FirstOrDefault().Connections.Last().ConnectionID;

            chatModel.UsersConnectionId = GetUserConnectionIdList(chat.Id, adminConnectionId);

            return chatModel;
        }


        public async Task<ChatViewModel> ReturnCreatedGroupAsync(CreateGroupModelView groupModel)
        {
            var chat = await CreateGroupAsync(groupModel);
            var chatModel = _mapper.Map<ChatViewModel>(chat);

            var adminConnectionId = _context.Users
                .Include(x => x.Connections)
                .Where(x => x.Id == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)
                .FirstOrDefault().Connections.Last().ConnectionID;

            chatModel.UsersConnectionId = GetUserConnectionIdList(chat.Id, adminConnectionId); 

            return chatModel;
        }

        public List<string> GetUserConnectionIdList(int chatId, string connectionId)
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
                .Select(c => c.Connections.Last().ConnectionID).Where(c => c != connectionId).ToList();

            return connectionIdList;
        }

        public User GetUser(string Id) =>
         _context.Users
                .Where(x => x.Id == Id)
                .FirstOrDefault();

        public List<UserViewModel> GetUsers()
        {
            var users = _context.Users
                  .Where(x => x.Id != _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();

            var userList = _mapper.Map<List<UserViewModel>>(users);

            userList.Sort((a, b) => string.Compare(a.UserName, b.UserName));
            return userList;
        }

        public ChatViewModel GetPrivateChat(string Id)
        {
            var chat = CheckPrivateChat(Id);
            return _mapper.Map<ChatViewModel>(chat);                
        }

        public string GetActiveUserName()
        {
            var name = _httpContextAccessor.HttpContext.User.Identity.Name;
            return name;
        }

    }
}
