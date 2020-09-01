using System;
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
using SignalROnlineChatServer.BLL.Services.Interfaces;
using SignalROnlineChatServer.DataBase;
using SignalROnlineChatServer.Hubs;
using SignalROnlineChatServer.Models;
using SignalROnlineChatServer.Models.ModelViews;

namespace SignalROnlineChatServer.Controllers
{
    [Authorize]
    //[Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly OnlineChatDBContext _context;
        private readonly IHomeService _homeService;
        private readonly IHubContext<HomeHub> _chat;
        private readonly IChatService _chatService;
        public HomeController(OnlineChatDBContext context, IHomeService service, IHubContext<HomeHub> chat, IChatService chatService) //
        {
            _context = context;
            _homeService = service;
            _chat = chat;
            _chatService = chatService;
        }

        [Route("Home/Home/Index")]
        [HttpGet]
        public IActionResult Index()
        {
            var myChats = _homeService.GetAllChats();
            return View("Index", myChats);            
        }
                

        [Route("GetChat")]
        [HttpGet("{id}")]
        public IActionResult GetChat(int id)
        {
            var chat = _homeService.GetChat(id);

            var chatView = _homeService.GetChatView(chat);

            return View(chatView);
        }

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

        [Route("Home/CheckPrivateChat")]
        [HttpGet("{Id}")]
        public IActionResult CheckPrivateChat(string Id)
        {
            var chat = _homeService.CheckPrivateChat(Id);

            if (chat == null)
            {
                var chatCreateInfo = new CreatePrivateChatViewModel()
                {
                    UserId = Id,
                    UserName = _homeService.GetUser(Id).UserName//GetUser(Id).UserName
                };

                return View("CreatePrivateChatSubmit", chatCreateInfo);
            }

            return RedirectToAction("GetChat", new { id = chat.Id });

        }


        [Route("Home/Home/CreateGroupAsync")]
        [HttpPost]
        public async Task<IActionResult> CreateGroupAsync(CreateGroupModelView groupModel, string connectionId)
        {
            var groupChat = await _homeService.ReturnCreatedGroupAsync(groupModel);
            await _chat.Groups.AddToGroupAsync(connectionId, groupChat.Name);

            foreach(var Id in groupChat.UsersConnectionId)
            {
                await _chat.Groups.AddToGroupAsync(Id, groupChat.Name);
            }

            await _chatService.IncreaseUsersUnreadMessageCount(groupChat.Id);
            // await _chat.Clients.Group(chatView.Name).SendAsync("ChatCreated", chatView);
            await _chat.Clients.Group(groupChat.Name).SendAsync("ChatCreated", groupChat);

            return Ok();

        }


        [Route("Home/Home/CreatePrivateChatAsync")] 
        [HttpPost]
        public async Task<IActionResult> CreatePrivateChatAsync(string Id, string connectionId) //[FromBody] CreatePrivateChatViewModel chatOptions
        {          

            var chatView = await _homeService.ReturnCreatedPrivateChatAsync(Id);

            await _chat.Groups.AddToGroupAsync(connectionId, chatView.Name);
            await _chat.Groups.AddToGroupAsync(chatView.UsersConnectionId.Last(), chatView.Name);

            await _chatService.IncreaseUsersUnreadMessageCount(chatView.Id);

            await _chat.Clients.Client(connectionId).SendAsync("GetCreatedChat", chatView.Id);
            await _chat.Clients.Group(chatView.Name).SendAsync("ChatCreated", chatView);

            //return View("PrivateChatCreatingNotification", chatView);
            return Ok();

        }

        [HttpGet]
        public IActionResult DisplayCreateGroupForm()
        {
            ViewData["Users"] = new SelectList(_homeService.GetUsers(), "Id", "UserName");
            return View("CreateGroup", new CreateGroupModelView());
        }

        [Route("FindUsers")]
        [HttpGet]
        public IActionResult FindUsers() => View("FindUsers", _homeService.GetUsers());
        
    }
}





//TODO
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

GetChat(){
//var chat = _context.Chats
            //    .Include(x => x.Messages)
            //    // .Include(y => y.ChatParticipants).ThenInclude(y => y.User)
            //    .FirstOrDefault(x => x.Id == id);

            //var chatView = new ChatViewModel();
}


CreateGroupAsync{
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
 */



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



//PRIVATECHATCREATINGNOTIFICATION functions

//[Route("Home/Home/ConnectToPrivateChat")]
//[HttpPost]
//public async Task<IActionResult> ConnectToPrivateChat(string connectionId, string groupName, string participantConId)
//{
//    await _chat.Groups.AddToGroupAsync(connectionId, groupName);
//    await _chat.Groups.AddToGroupAsync(participantConId, groupName);
//    return Ok();
//}

//[Route("Home/Home/SendNotificationAboutCreatingChat")] //, Name ="createPrivateChat"
//[HttpPost]
//public async Task<IActionResult> SendNotificationAboutCreatingChat(int chatId, string chatName) //[FromBody] CreatePrivateChatViewModel chatOptions
//{
//    var chatView = _homeService.GetChat(chatId);

//    //await _chat.Groups.AddToGroupAsync(, chatView.Name);

//    //var chatView = _homeService.GetPrivateChat(Id);

//    await _chat.Clients.Group(chatName).SendAsync("ChatCreated", chatView);

//    //return View("GetChat",chatView);


//    //return RedirectToAction("GetChat", new { id = chatView.Id });

//    return Ok();
//    //return View("GetChat",chatView);

//}