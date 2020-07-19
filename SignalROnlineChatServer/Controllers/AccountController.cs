using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignalROnlineChatServer.Models;
using SignalROnlineChatServer.Models.ModelViews;
//using SignalROnlineChatServer.Models.ModelViews;

namespace SignalROnlineChatServer.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Route("DisplayLogin")]
        [HttpGet]
        public IActionResult DisplayLogin() => View("Login", new LoginViewModel());

        [Route("Account/LoginAsync")]
        [HttpPost]//("{login}/{password}")
        public async Task<IActionResult> LoginAsync(LoginViewModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.Login);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("DisplayLogin", "Account");
        }

        [Route("DisplayRegister")]
        [HttpGet]
        public IActionResult DisplayRegister() => View("Register", new RegisterViewModel());

        [Route("Account/RegisterAsync")]
        [HttpPost]//("{login}/{password}")
        public async Task<IActionResult> RegisterAsync(RegisterViewModel registerModel) //[FromHeader(Name ="login")]string login, [FromHeader(Name = "password")] string password
        {
            var user = new User
            {
                UserName = registerModel.Login
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);

                return RedirectToAction("Index", "Home");
            }

            //return 
            return RedirectToAction("DisplayRegister", "Account");
        }

        [Route("Logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("LoginAsync", "Account");
        }
    }
}
