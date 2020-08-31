using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalROnlineChatServer.BLL.Services;
using SignalROnlineChatServer.BLL.Services.Interfaces;
using SignalROnlineChatServer.DataBase;
using SignalROnlineChatServer.Hubs;
using SignalROnlineChatServer.Models;
using SignalROnlineChatServer.Models.ModelViews;

namespace SignalROnlineChatServer.Controllers
{
    [Authorize]
   // [Route("[controller]")]
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> _chat;
        private readonly IHubContext<HomeHub> _homePage;
        //private readonly IHomeService _homeService;
        private readonly IChatService _chatService;
        private readonly IMapper _mapper;
        public ChatController(IHubContext<ChatHub> chat, IHubContext<HomeHub> homePage, IChatService chatService, IMapper mapper)
        {
            _chat = chat;
            _chatService = chatService;
            _mapper = mapper;
            _homePage = homePage;
        }

        [Route("Chat/JoinChatAsync")]
        [HttpPost]
        public async Task<IActionResult> JoinChatAsync(string connectionId, string groupName)
        {
            await _chat.Groups.AddToGroupAsync(connectionId, groupName);
            return Ok();
        }


        //[HttpPost("[action]/{connectionId}/{groupName}")]
        //public async Task<IActionResult> LeaveChatAsync(string connectionId, string groupName)
        //{
        //    await _chat.Groups.RemoveFromGroupAsync(connectionId, groupName);
        //    return Ok();
        //}

        [Route("Chat/SendMessageAsync")]
        [HttpPost]
        public async Task<IActionResult> SendMessageAsync(int groupId, string message, string groupName, string connectionId,[FromServices] OnlineChatDBContext context)
        {
            //var newMessage = new Message
            //{
            //    ChatId = groupId,
            //    Timestamp = DateTime.Now,
            //    Text = message,
            //    Name = User.Identity.Name
            //};

            //context.Messages.Add(newMessage);
            //await context.SaveChangesAsync();

            //var messageView = _mapper.Map<MessageViewModel>(newMessage);
            var messageView = await _chatService.ReturnSendedMessageAsync(groupId, message);
            //messageView.Type = _homeService.CheckMessagesType(messageView);

            await _chat.Clients.Group(groupName)
                .SendAsync("ReceiveMessage", messageView, connectionId, groupId);

            await NotificateUsers(groupId, connectionId, messageView);

            //var connectionIdList = _homeService.GetUserConnectionIdList(groupId, connectionId);

            //await _homePage.Clients.Clients(connectionIdList).SendAsync("PushNotification", messageView, groupId);

            //connectionIdList.Add(connectionId);

            //await _homePage.Clients.Clients(connectionIdList).SendAsync("UpdateLastMessage", messageView, groupId);


            //await _homePage.Clients.All/*.GroupExcept(groupName, connectionId)*/
            //   .SendAsync("PushNotification", messageView);

            //.SendAsync("ReceiveMessage", new
            //{
            //    Text = newMessage.Text,
            //    Name = newMessage.Name,
            //    Timestamp = newMessage.Timestamp.ToString("hh:mm | d MMM")
            //});

            return Ok();
        }

        public async Task<IActionResult> NotificateUsers(int groupId, string connectionId, MessageViewModel messageView)
        {
            var connectionIdList = _chatService.GetUserConnectionIdList(groupId, connectionId);

            await _chatService.IncreaseUsersUnreadMessageCount(groupId);
            await _chatService.ReduceUserUnreadMessageCount(groupId);

            await _homePage.Clients.Clients(connectionIdList).SendAsync("PushNotification", groupId);
            await _homePage.Clients.Client(connectionId).SendAsync("ClearNotification", groupId);

            connectionIdList.Add(connectionId);

            await _homePage.Clients.Clients(connectionIdList).SendAsync("UpdateLastMessage", messageView, groupId);
            return Ok();
        }

        public void AddUnreadMessage(int groupId, [FromServices] OnlineChatDBContext context)
        {
            //var chat = _homeService.GetChat(groupId);
            //chat.UnreadMessages++;
            //context.
        }

    }
}
