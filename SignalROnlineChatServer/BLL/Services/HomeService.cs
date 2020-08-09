﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SignalROnlineChatServer.DataBase;
using SignalROnlineChatServer.Models;
using SignalROnlineChatServer.Models.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
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
            var chats = _context.Chats
                .Include(x => x.ChatParticipants).ThenInclude(x => x.User)
                .Include(x => x.Messages)
                .Where(x => x.ChatParticipants.Any(y => y.UserId == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value))
                .ToList();

            var myChats = new List<ChatViewModel>();

            foreach (Chat chat in chats)
            {
                if (chat.Messages.Count() != 0)
                {
                    var chatModel = _mapper.Map<ChatViewModel>(chat);
                    myChats.Add(chatModel);
                }
            }

            return myChats;
        }


        public async void CreateGroupAsync(CreateGroupModelView groupModel, string CreatorId)
        {
            var chat = new Chat
            {
                Name = groupModel.Name,
                Type = ChatType.Group
            };

            chat.ChatParticipants.Add(new ChatUser
            {
                UserId = _context.Users
                            .Where(x => x.Id == CreatorId).FirstOrDefault().Id,
                Role = UserRole.Admin
            });

            foreach (var userId in groupModel.ChatParticipantsId)
            {
                chat.ChatParticipants.Add(new ChatUser
                {
                    UserId = _context.Users
                        .Where(x => x.Id == userId).FirstOrDefault().Id,
                    Role = UserRole.Member
                });
            }

            _context.Chats.Add(chat);

            await _context.SaveChangesAsync();
        }



        public ChatViewModel GetChat(int id)
        {
            var chat = _context.Chats
                .Include(x => x.Messages)
                // .Include(y => y.ChatParticipants).ThenInclude(y => y.User)
                .FirstOrDefault(x => x.Id == id);

            //NEED DELETED///////////
            //if(chat.Messages.Count == 0)
            //{
            //    chat.Messages.Add(new Message()
            //    {
            //        ChatId = chat.Id,
            //        Timestamp = DateTime.Now,
            //        Text = chat.Name + " created",
            //        Name = "Default"
            //    }) ;
            //}
            //////////////////////////

            var chatView = _mapper.Map<ChatViewModel>(chat);

            //chatView = CheckMessagesType(chatView);
            foreach(var message in chatView.Messages)
            {
                message.Type = CheckMessagesType(message);
            }
            return chatView;
        }

        public MessageType CheckMessagesType(MessageViewModel model)
        {
            var activeAccount = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
          
            if (model.Name == activeAccount) return MessageType.Outgoing;
            else if (model.Name == "Default") return MessageType.Default;
            else return MessageType.Incoming;
            
        }


        //public ChatViewModel PrepareChatModelForView(ChatViewModel model)
        //{
        //    model.LastMessage = model.Messages.Last().Text;
        //    model.LastMessageDate = model.Messages.Last().Timestamp.Date.ToString();

        //    var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
        //    foreach (var message in model.Messages)
        //    {
        //        if (message.Name == userName)
        //        {
        //            model.MessageType
        //        }
        //    }
        //}

        public async void CreateMessageAsync(int groupId, string message)
        {

            var newMessage = new Message
            {
                ChatId = groupId,
                Timestamp = DateTime.Now,
                Text = message,
                //Name = User.Identity.Name
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();
            //GetChat(groupId);
        }

        //[Route("FindUsers")]
        //[HttpGet]
        //public IActionResult FindUsers() => View("FindUsers", GetUsers());

        public Chat CheckPrivateChat(string Id/*, string ActiveUserId*/)
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

        public async Task<Chat> CreatePrivateChatAsync(string ParticipantId/*, string CreatorId*/) //[FromBody] CreatePrivateChatViewModel chatOptions
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

        public async Task<ChatViewModel> ReturnCreatedPrivateChatAsync(string ParticipantId)
        {
            var chat = await CreatePrivateChatAsync(ParticipantId);

            var chatModel = _mapper.Map<ChatViewModel>(chat);

            return chatModel;
        }

        public User GetUser(string Id) =>
         _context.Users
                .Where(x => x.Id == Id)
                .FirstOrDefault();

        public List<UserViewModel> GetUsers()
        {
            var users = _context.Users
                  .Where(x => x.Id != _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userList = new List<UserViewModel>();

            //userList = _mapper.Map<List<UserViewModel>>(users);

            foreach (User user in users)
            {
                var userModel = _mapper.Map<UserViewModel>(user);
                userList.Add(userModel);
            }

            userList.Sort((a, b) => string.Compare(a.UserName, b.UserName));
            return userList;
        }

        public ChatViewModel GetPrivateChat(string Id)
        {
            var chat = CheckPrivateChat(Id);
            return _mapper.Map<ChatViewModel>(chat);
                
        }
    }
}



////TODO
//[Route("Home/JoinGroup")]
//[HttpPost] //("{id}")
//public async Task<IActionResult> JoinGroupAsync(int id)
//{
//    var chatMember = new ChatUser
//    {
//        ChatId = id,
//        UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
//        Role = UserRole.Member
//    };

//    _context.ChatUsers.Add(chatMember);

//    await _context.SaveChangesAsync();

//    return RedirectToAction("GetChat", "Home"); // new {id = id}
//}
