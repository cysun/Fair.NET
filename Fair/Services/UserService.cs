using System.Linq;
using System.Security.Claims;
using Fair.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;

namespace Fair.Services
{
    public class UserService
    {
        private readonly AppDbContext db;

        private readonly IADService adService;

        private readonly ILogger<UserService> logger;

        public UserService(AppDbContext db, IADService adService, ILogger<UserService> logger)
        {
            this.db = db;
            this.adService = adService;
            this.logger = logger;
        }

        public ClaimsIdentity Authenticate(string username, string password)
        {
            var user = GetUser(username);
            if (user == null || !adService.Authenticate(username, password))
                return null;

            var claims = user.ToClaims();
            return new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public User GetUser(string username)
        {
            return db.Users.Where(u => u.Username.ToUpper() == username.ToUpper()).SingleOrDefault();
        }

        public void AddUser(User user)
        {
            db.Users.Add(user);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}
