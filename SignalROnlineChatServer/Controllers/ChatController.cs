using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalROnlineChatServer.DataBase;
using SignalROnlineChatServer.Hubs;
using SignalROnlineChatServer.Models;

namespace SignalROnlineChatServer.Controllers
{
    [Authorize]
    //[Route("[controller]")]
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> _chat;
        public ChatController(IHubContext<ChatHub> chat)
        {
            _chat = chat;
        }

        [Route("Home/Chat/JoinChatAsync")]
        [HttpPost]
        public async Task<IActionResult> JoinChatAsync(string connectionId, string groupName)
        {
            await _chat.Groups.AddToGroupAsync(connectionId, groupName);
            return Ok();
        }


        [HttpPost("[action]/{connectionId}/{groupName}")]
        public async Task<IActionResult> LeaveChatAsync(string connectionId, string groupName)
        {
            await _chat.Groups.RemoveFromGroupAsync(connectionId, groupName);
            return Ok();
        }

        [Route("Home/Chat/SendMessageAsync")]
        [HttpPost]
        public async Task<IActionResult> SendMessageAsync(int groupId, string message, string groupName, [FromServices] OnlineChatDBContext context)
        {
            var newMessage = new Message
            {
                ChatId = groupId,
                Timestamp = DateTime.Now,
                Text = message,
                Name = User.Identity.Name
            };

            context.Messages.Add(newMessage);
            await context.SaveChangesAsync();

            //await _chat.Clients.Group(groupName)
            //    .SendAsync("ReceiveMessage", newMessage);


            await _chat.Clients.Group(groupName)
                .SendAsync("ReceiveMessage", new
                {
                    Text = newMessage.Text,
                    Name = newMessage.Name,
                    Timestamp = newMessage.Timestamp.ToString("hh:mm | d MMM")

                });

            return Ok();
        }

    }
}
