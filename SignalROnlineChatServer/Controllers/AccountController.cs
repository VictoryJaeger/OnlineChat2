using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalROnlineChatServer.Hubs;
using SignalROnlineChatServer.Models;
using SignalROnlineChatServer.Models.ModelViews;

namespace SignalROnlineChatServer.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHubContext<ChatHub> _chat;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IHubContext<ChatHub> chat)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _chat = chat;
        }

        [HttpGet]
        public IActionResult DisplayLogin() => View("Login", new LoginViewModel());

        [Route("Account/LoginAsync")]
        [HttpPost]
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

            ModelState.AddModelError("", "Incorrect login or password");

            return View("Login", loginModel);
        }

        [Route("DisplayRegister")]
        [HttpGet]
        public IActionResult DisplayRegister() => View("Register", new RegisterViewModel());

        [Route("Account/RegisterAsync")]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel registerModel) 
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = registerModel.Login,
                    Connections = new List<Connection>()
                };

                var result = await _userManager.CreateAsync(user, registerModel.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);           

                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "Error! Please, try another name");
            return View("Register", registerModel);

        }

        [Route("Account/LogoutAsync")]
        public async Task<IActionResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("DisplayLogin", "Account");
        }
    }
}
