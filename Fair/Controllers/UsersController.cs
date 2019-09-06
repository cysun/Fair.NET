using System.Collections.Generic;
using System.Security.Claims;
using Fair.Models;
using Fair.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fair.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserService userService;
        private readonly ILogger<UsersController> logger;

        public UsersController(UserService userService, ILogger<UsersController> logger)
        {
            this.userService = userService;
            this.logger = logger;
        }

        public IActionResult List()
        {
            return View(userService.GetUsers());
        }

        [HttpGet]
        [Authorize("IsSysAdmin")]
        public IActionResult Add()
        {
            return View(new User());
        }

        [HttpPost]
        [Authorize("IsSysAdmin")]
        public IActionResult Add(User user)
        {
            userService.AddUser(user);
            userService.SaveChanges();

            logger.LogInformation("{username} added user {name}", User.FindFirst(ClaimTypes.Name).Value, user.Name);

            return RedirectToAction(nameof(List));
        }

        [HttpGet("/api/users/search")]
        public List<User> Search([FromQuery(Name = "q")]string prefix)
        {
            return userService.SearchByPrefix(prefix);
        }
    }
}
