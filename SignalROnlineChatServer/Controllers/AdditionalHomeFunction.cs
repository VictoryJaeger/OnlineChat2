using Microsoft.AspNetCore.Mvc;
using SignalROnlineChatServer.DataBase;
using SignalROnlineChatServer.Models;
using SignalROnlineChatServer.Models.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Controllers
{
    public class AdditionalHomeFunction
    {
        private readonly OnlineChatDBContext _context;

        public AdditionalHomeFunction(OnlineChatDBContext context)
        {
            _context = context;
        }
        public User GetUser(string Id) =>
        _context.Users
               .Where(x => x.Id == Id)
               .FirstOrDefault();

        //public IEnumerable<UserViewModel> GetUsers()
        //{
        //    var users = _context.Users
        //          .Where(x => x.Id != User.FindFirst(ClaimTypes.NameIdentifier).Value);
        //    var userList = new List<UserViewModel>();

        //    foreach (User user in users)
        //    {
        //        userList.Add(new UserViewModel(user.UserName, user.Id));
        //    }

        //    return userList;

        //}
    }
}
