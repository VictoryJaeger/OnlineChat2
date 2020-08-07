﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalROnlineChatServer.BLL.Services;
using SignalROnlineChatServer.DataBase;
using SignalROnlineChatServer.Hubs;
using SignalROnlineChatServer.Models;
using SignalROnlineChatServer.Models.ModelViews;

namespace SignalROnlineChatServer.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly OnlineChatDBContext _context;
        private readonly IHomeService _homeService;
        //private readonly IHubContext<ChatHub> _chat;
        public HomeController(OnlineChatDBContext context, IHomeService service) //, IHubContext<ChatHub> chat
        {
            _context = context;
            _homeService = service;
            //_chat = chat;
        }

        [Route("Index")]
        [HttpGet]
        public IActionResult Index()
        {
            //var chats = _context.Chats
            //    .Include(x => x.ChatParticipants).ThenInclude(x => x.User)
            //    .Where(x => x.ChatParticipants.Any(y => y.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value))
            //    .ToList();

            //var myChats = new List<ChatViewModel>();

            //foreach (Chat chat in chats)
            //{
            //    myChats.Add(new ChatViewModel(chat.Id, chat.Messages, chat.ChatParticipants, chat.Name));
            //}
            //var userId = _context.Users.Where(x => x.Id == User.FindFirst(ClaimTypes.NameIdentifier).Value).FirstOrDefault().Id;

            var myChats = _homeService.GetAllChats(/*userId*/);

            return View("Index", myChats);
            //return Ok();
        }



        [HttpGet]
        public IActionResult DisplayCreateGroupForm()
        {
            ViewData["Users"] = new SelectList(_homeService.GetUsers(), "Id", "UserName");
            return View("CreateGroup", new CreateGroupModelView());
        }

        [Route("CreateGroupAsync")]
        [HttpPost]
        public async Task<IActionResult> CreateGroupAsync(CreateGroupModelView groupModel)
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
            //var chat = _context.Chats
            //    .Include(x => x.Messages)
            //    // .Include(y => y.ChatParticipants).ThenInclude(y => y.User)
            //    .FirstOrDefault(x => x.Id == id);

            //var chatView = new ChatViewModel(/*chat.Id, chat.Messages, chat.ChatParticipants, chat.Name*/);
            var chatView = _homeService.GetChat(id);

            return View(chatView);
        }

        //[Route("Home/CreateMessageAsync")]
        //[HttpPost]
        //public async Task<IActionResult> CreateMessageAsync(int groupId, string message, string groupName)
        //{

        //    var newMessage = new Message
        //    {
        //        ChatId = groupId,
        //        Timestamp = DateTime.Now,
        //        Text = message,
        //        Name = User.Identity.Name
        //    };

        //    _context.Messages.Add(newMessage);
        //    await _context.SaveChangesAsync();

        //    await _chat.Clients.Group(groupName)  
        //        .SendAsync("ReceiveMessage", newMessage);

        //    return Ok();
        //    //return RedirectToAction("GetChat", new { id = groupId });
        //}


        [Route("Home/CreateMessageAsync")]
        [HttpPost]
        public async Task<IActionResult> CreateMessageAsync(int groupId, string message)
        {

            var newMessage = new Message
            {
                ChatId = groupId,
                Timestamp = DateTime.Now,
                Text = message,
                Name = User.Identity.Name
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            return RedirectToAction("GetChat", new { id = groupId });
        }

        [Route("FindUsers")]
        [HttpGet]
        public IActionResult FindUsers() => View("FindUsers", _homeService.GetUsers());

        [Route("Home/CheckPrivateChat")]
        [HttpGet("{Id}")]
        public IActionResult CheckPrivateChat(string Id)
        {
            var chat = _context.Chats
                .Include(x => x.ChatParticipants).ThenInclude(x => x.User)
                .Where(x => x.Type == ChatType.Private &&
                 x.ChatParticipants.Any(y => y.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value
                 && x.ChatParticipants.Any(y => y.UserId == _context.Users.Where(x => x.Id == Id).FirstOrDefault().Id)))
                .FirstOrDefault();

            if (chat == null)
            {
                var chatCreateInfo = new CreatePrivateChatViewModel()
                {
                    UserId = Id,
                    UserName = GetUser(Id).UserName
                };

                return View("CreatePrivateChatSubmit", chatCreateInfo);
            }

            return RedirectToAction("GetChat", new { id = chat.Id });

        }


        //[AllowAnonymous]
        [Route("CreatePrivateChatAsync")] //, Name ="createPrivateChat"
        [HttpPost]
        public async Task<IActionResult> CreatePrivateChatAsync(string Id) //[FromBody] CreatePrivateChatViewModel chatOptions
        {          

            var chat = await _homeService.CreatePrivateChatAsync(Id);

            return RedirectToAction("GetChat", new { id = chat.Id });

            //return Ok();

        }

        public User GetUser(string Id) =>
         _context.Users
                .Where(x => x.Id == Id)
                .FirstOrDefault();

        
    }
}





/*
 public IEnumerable<UserViewModel> GetUsers()
        {
            var users = _context.Users
                  .Where(x => x.Id != User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userList = new List<UserViewModel>();

            foreach (User user in users)
            {
                userList.Add(new UserViewModel());
            }

            return userList;

        }
 */

//[Route("GetPrivateChats")]
//[HttpGet]
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
//        chatViews.Add(new ChatViewModel(/*chat.Id, chat.Messages, chat.ChatParticipants, chat.Name*/));
//    }

//    var myChats = new UserChatsViewModel(chatViews);

//    return View("GetPrivateChats", myChats);
//}

/*
 CreatePrivateChat(){
 //var chat = new Chat
            //{
            //    Type = ChatType.Private,
            //    Name = _context.Users.Where(x => x.Id == Id).FirstOrDefault().UserName + ',' +
            //        _context.Users.Where(x => x.Id == User.FindFirst(ClaimTypes.NameIdentifier).Value).FirstOrDefault().UserName
            //};

            //chat.ChatParticipants.Add(new ChatUser
            //{
            //    UserId = Id
            //});

            //chat.ChatParticipants.Add(new ChatUser
            //{
            //    UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value
            //});

            //_context.Chats.Add(chat);

            //await _context.SaveChangesAsync();
}
 */
