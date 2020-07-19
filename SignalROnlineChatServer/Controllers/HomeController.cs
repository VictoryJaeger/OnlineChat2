using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalROnlineChatServer.DataBase;
using SignalROnlineChatServer.Models;
using SignalROnlineChatServer.Models.ModelViews;

namespace SignalROnlineChatServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly OnlineChatDBContext _context;

        public HomeController(OnlineChatDBContext context)
        {
            _context = context;
        }

        [Route("Index")]
        [HttpGet]
        public IActionResult Index()
        {
            var chats = _context.Chats
                .Include(x => x.ChatParticipants).ThenInclude(x => x.User)
                .Where(x => x.ChatParticipants.Any(y => y.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value))
                .ToList();

            var myChats = new List<ChatViewModel>();

            foreach (Chat chat in chats)
            {
                myChats.Add(new ChatViewModel(chat.Id, chat.Messages, chat.ChatParticipants, chat.Name));
            }

            return View("Index", myChats);
        }

        [Route("GetPrivateChats")]
        [HttpGet]
        public IActionResult GetPrivateChats()
        {
            var chats = _context.Chats
                .Include(x => x.ChatParticipants).ThenInclude(x => x.User)
                .Where(x => x.Type == ChatType.Private
                    && x.ChatParticipants.Any(y => y.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value))
                .ToList();

            var chatViews = new List<ChatViewModel>();

            foreach (Chat chat in chats)
            {
                chatViews.Add(new ChatViewModel(chat.Id, chat.Messages, chat.ChatParticipants, chat.Name));
            }

            var myChats = new UserChatsViewModel(chatViews);

            return View("GetPrivateChats", myChats);
        }

        [Route("CreateGroup")]
        [HttpPost]
        public async Task<IActionResult> CreateGroupAsync([FromBody] CreateGroupModelView groupModel)
        {
            var chat = new Chat
            {
                Name = groupModel.Name,
                Type = ChatType.Group
            };

            chat.ChatParticipants.Add(new ChatUser
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Role = UserRole.Admin
            });


            _context.Chats.Add(chat);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        //TODO
        [Route("Home/JoinGroup")]
        [HttpPost] //("{id}")
        public async Task<IActionResult> JoinGroupAsync(int id)
        {
            var chatMember = new ChatUser
            {
                ChatId = id,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Role = UserRole.Member
            };

            _context.ChatUsers.Add(chatMember);

            await _context.SaveChangesAsync();

            return RedirectToAction("GetChat", "Home"); // new {id = id}
        }


        [Route("GetChat")]
        [HttpGet("{id}")]
        public IActionResult GetChat(int id)
        {
            var chat = _context.Chats
                .Include(x => x.Messages)
               // .Include(y => y.ChatParticipants).ThenInclude(y => y.User)
                .FirstOrDefault(x => x.Id == id);

            var chatView = new ChatViewModel(chat.Id, chat.Messages, chat.ChatParticipants, chat.Name);

            return View(chatView);
        }

        [Route("CreateMessage")]
        [HttpPost]
        public async Task<IActionResult> CreateMessageAsync([FromBody] MessageViewModel messageModel)
        {

            var message = new Message
            {
                ChatId = messageModel.ChatId,
                Timestamp = DateTime.Now,
                Text = messageModel.Text,
                Name = User.Identity.Name
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("GetChat", new { id = messageModel.ChatId });
        }

        [Route("FindUsers")]
        [HttpGet]
        public IActionResult FindUsers()
        {
            IEnumerable<User> users = _context.Users
                .Where(x => x.Id != User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //.ToList();

            var userList = new List<UserViewModel>();

            foreach (User user in users)
            {
                userList.Add(new UserViewModel(user.UserName, user.Id));
            }

            return View("FindUsers", userList);
        }

        [Route("CreatePrivateChat")]
        [HttpPost]

        public async Task<IActionResult> CreatePrivateChatAsync([FromBody] CreatePrivateChatViewModel chatOptions)
        {

            var chat = new Chat
            {
                Type = ChatType.Private,
                Name = _context.Users.Where(x => x.Id == chatOptions.UserId).FirstOrDefault().UserName
            };

            chat.ChatParticipants.Add(new ChatUser
            {
                UserId = chatOptions.UserId
            });

            chat.ChatParticipants.Add(new ChatUser
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value
            }) ;

            _context.Chats.Add(chat);

            await _context.SaveChangesAsync();

            return RedirectToAction("GetChat", new {id = chat.Id });

        }

    }
}
