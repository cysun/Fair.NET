using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Fair.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fair.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserService userService;

        private readonly ILogger<AccountController> logger;

        public AccountController(UserService userService, ILogger<AccountController> logger)
        {
            this.userService = userService;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return User.Identity.IsAuthenticated ? RedirectToAction("List", "Searches") : (IActionResult)View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl)
        {
            var identity = userService.Authenticate(username, password);
            if (identity == null)
                return RedirectToAction(nameof(Login), new { failed = true });

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true
                });

            logger.LogInformation("User {username} signed in at {timestamp}", username, DateTime.Now);

            return string.IsNullOrWhiteSpace(returnUrl) ?
                RedirectToAction("List", "Searches") :
                (IActionResult)LocalRedirect(returnUrl);
        }

        public async Task<IActionResult> Logout()
        {
            logger.LogInformation("User {username} signed out at {timestamp}", User.Identity.Name, DateTime.Now);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
