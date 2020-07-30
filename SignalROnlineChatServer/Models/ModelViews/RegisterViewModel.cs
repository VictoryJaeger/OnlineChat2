using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Models.ModelViews
{

    [BindProperties(SupportsGet =true)]
    public class RegisterViewModel
    {
        //public RegisterViewModel(string login, string password)
        //{
        //    Login = login;
        //    Password = password;
        //}
        //[BindNever]
        public int Id { get; set; }

        [Required]
        //[BindProperty]
        public string Login { get; set; }

        //[BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Minimal size - 6 symbols ")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        [DataType(DataType.Password)]
        //[Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }
    }
}
