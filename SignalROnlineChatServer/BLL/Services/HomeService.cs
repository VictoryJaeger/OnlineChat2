using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SignalROnlineChatServer.DataBase;
using SignalROnlineChatServer.Models;
using SignalROnlineChatServer.Models.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    //myChats.Add(new ChatViewModel(/*chat.Id, chat.Messages, chat.ChatParticipants, chat.Name*/));
                }
            }

            return myChats;
        }



        //public IActionResult GetPrivateChats()
        //{
        //    var chats = _context.Chats
        //        .Include(x => x.ChatParticipants).ThenInclude(x => x.User)
        //        .Where(x => x.Type == ChatType.Private
        //            && x.ChatParticipants.Any(y => y.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value))
        //        .ToList();

        //    var chatViews = new List<ChatViewModel>();

        //    foreach (Chat chat in chats)
        //    {
        //        chatViews.Add(new ChatViewModel(chat.Id, chat.Messages, chat.ChatParticipants, chat.Name));
        //    }

        //    var myChats = new UserChatsViewModel(chatViews);

        //    return View("GetPrivateChats", myChats);
        //}

        //public IActionResult DisplayCreateGroupForm()
        //{
        //    ViewData["Users"] = new SelectList(GetUsers(), "Id", "UserName");
        //    return View("CreateGroup", new CreateGroupModelView());
        //}

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


        public ChatViewModel GetChat(int id)
        {
            var chat = _context.Chats
                .Include(x => x.Messages)
                // .Include(y => y.ChatParticipants).ThenInclude(y => y.User)
                .FirstOrDefault(x => x.Id == id);

            //var chatView = new ChatViewModel(/*chat.Id, chat.Messages, chat.ChatParticipants, chat.Name*/);
            var chatView = _mapper.Map<ChatViewModel>(chat);
            chatView = CheckMessagesType(chatView);
            return chatView;
        }

        public ChatViewModel CheckMessagesType(ChatViewModel model)
        {
            var activeAccount = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            foreach (var message in model.Messages)
            {
                if (message.Name == activeAccount) message.Type = MessageType.Outgoing;
                else message.Type = MessageType.Incoming;
            }
            return model;
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

        public bool CheckPrivateChat(string Id, string ActiveUserId)
        {
            var chat = _context.Chats
                .Include(x => x.ChatParticipants).ThenInclude(x => x.User)
                .Where(x => x.Type == ChatType.Private &&
                 x.ChatParticipants.Any(y => y.UserId == _context.Users.Where(x => x.Id == ActiveUserId).FirstOrDefault().Id
                 && x.ChatParticipants.Any(y => y.UserId == _context.Users.Where(x => x.Id == Id).FirstOrDefault().Id)))
                .FirstOrDefault();

            if (chat == null) return false;

            return true;

        }

        public async void CreatePrivateChatAsync(string ParticipantId, string CreatorId) //[FromBody] CreatePrivateChatViewModel chatOptions
        {

            var chat = new Chat
            {
                Type = ChatType.Private,
                Name = _context.Users.Where(x => x.Id == CreatorId).FirstOrDefault().UserName + ',' +
                       _context.Users.Where(x => x.Id == ParticipantId).FirstOrDefault().UserName
            };

            chat.ChatParticipants.Add(new ChatUser
            {
                UserId = ParticipantId
            });

            chat.ChatParticipants.Add(new ChatUser
            {
                UserId = CreatorId
            });

            _context.Chats.Add(chat);

            await _context.SaveChangesAsync();
            //GetChat(chat.Id);
        }

        public User GetUser(string Id) =>
         _context.Users
                .Where(x => x.Id == Id)
                .FirstOrDefault();

        public IEnumerable<UserViewModel> GetUsers(string ActiveUserId)
        {
            var users = _context.Users
                  .Where(x => x.Id != _context.Users.Where(x => x.Id == ActiveUserId).FirstOrDefault().Id);
            var userList = new List<UserViewModel>();

            foreach (User user in users)
            {
                userList.Add(new UserViewModel(user.UserName, user.Id));
            }

            return userList;
        }
    }
}
