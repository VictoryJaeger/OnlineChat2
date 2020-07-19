using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Models.ModelViews
{
    [BindProperties(SupportsGet = true)]
    public class LoginViewModel
    {
        //public LoginViewModel(string login, string password)
        //{
        //    Login = login;
        //    Password = password;
        //}
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
