using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalROnlineChatServer.DataBase;
using SignalROnlineChatServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.ViewComponents
{
    public class ChatViewComponent: ViewComponent
    {
        private OnlineChatDBContext _context;

        public ChatViewComponent(OnlineChatDBContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var chats = _context.ChatUsers
                .Include(x => x.Chat)
                .Where(x => x.UserId == userId 
                && x.Chat.Type == ChatType.Group)
                .Select(x => x.Chat)
                .ToList();

            return View(chats);
        }
    }
}
