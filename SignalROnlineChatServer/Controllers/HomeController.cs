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

        [Route("/Home/Index")]
        [HttpGet]
        public IActionResult Index()
        {
            var myChats = _homeService.GetAllChats();
            return View("Index", myChats);            
        }
                

        [Route("/Home/GetChat")]
        [HttpGet("{id}")]
        public IActionResult GetChat(int id)
        {
            var chat = _homeService.GetChat(id);

            var chatView = _homeService.GetChatView(chat);

            return View(chatView);
        }

        [HttpDelete("{id}")]
        [Route("Home/DeleteChatAsync")]        
        public async Task<IActionResult> DeleteChatAsync(int chatId)
        {
            var connectionIdList = _chatService.GetUserConnectionIdList(chatId);
            await _chat.Clients.Clients(connectionIdList).SendAsync("DeleteChat", chatId);

            await _homeService.DeleteChat(chatId);
            await _chat.Clients.Clients(connectionIdList).SendAsync("ReturnOnHomePage");

            return RedirectToAction("Index");
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
                    UserName = _homeService.GetUser(Id).UserName
                };

                return View("CreatePrivateChatSubmit", chatCreateInfo);
            }

            return RedirectToAction("GetChat", new { id = chat.Id });

        }


        [Route("/Home/CreateGroupAsync")]
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
            await _chat.Clients.Group(groupChat.Name).SendAsync("ChatCreated", groupChat);

            return Ok();
        }


        [Route("/Home/CreatePrivateChatAsync")] 
        [HttpPost]
        public async Task<IActionResult> CreatePrivateChatAsync(string Id, string connectionId)
        {
            if (ModelState.IsValid)
            {
                var chatView = await _homeService.ReturnCreatedPrivateChatAsync(Id);

                await _chat.Groups.AddToGroupAsync(connectionId, chatView.Name);
                await _chat.Groups.AddToGroupAsync(chatView.UsersConnectionId.Last(), chatView.Name);

                await _chatService.IncreaseUsersUnreadMessageCount(chatView.Id);

                await _chat.Clients.Client(connectionId).SendAsync("GetCreatedChat", chatView.Id);
                await _chat.Clients.Group(chatView.Name).SendAsync("ChatCreated", chatView);

                return Ok();
            }
            return Json(ModelState);

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