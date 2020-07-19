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
    [Route("[controller]")]
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> _chat;
        public ChatController(IHubContext<ChatHub> chat)
        {
            _chat = chat;
        }

        [HttpPost("[action]/{connectionId}/groupId")]
        public async Task<IActionResult> JoinGroupAsync(string connectionId, string groupId)
        {
            await _chat.Groups.AddToGroupAsync(connectionId, groupId);
            return Ok();
        }

        [HttpPost("[action]/{connectionId}/groupId")]
        public async Task<IActionResult> LeaveGroupAsync(string connectionId, string groupId)
        {
            await _chat.Groups.RemoveFromGroupAsync(connectionId, groupId);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessageAsync(int groupId, string message, [FromServices] OnlineChatDBContext context)
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

            await _chat.Clients.Group(groupId.ToString())
                .SendAsync("ReceiveMessage", newMessage);


            //await _chat.Clients.Group(groupId)
            //    .SendAsync("ReceiveMessage", new {
            //        Text = newMessage.Text,
            //        Name = newMessage.Name,
            //        Timestamp = newMessage.Timestamp.ToString("dd/MM/yyyy hh:smm:ss")

            //    });

            return Ok();
        }

    }
}
