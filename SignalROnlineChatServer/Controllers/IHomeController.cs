using Microsoft.AspNetCore.Mvc;
using SignalROnlineChatServer.Models.ModelViews;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Controllers
{
    public interface IHomeController
    {
        IActionResult CheckPrivateChat(string Id);
        Task<IActionResult> CreateGroupAsync(CreateGroupModelView groupModel);
        Task<IActionResult> CreateMessageAsync(int groupId, string message);
        Task<IActionResult> CreatePrivateChatAsync(string Id);
        IActionResult DisplayCreateGroupForm();
        IActionResult FindUsers();
        IActionResult GetChat(int id);
        IActionResult GetPrivateChats();
        IActionResult Index();
        Task<IActionResult> JoinGroupAsync(int id);
    }
}