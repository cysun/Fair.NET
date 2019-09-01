using System.Collections.Generic;
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
            if (user == null)
            {
                logger.LogInformation("User authentication failed for {username}", username);
                return null;
            }

            if (!adService.Authenticate(username, password))
            {
                logger.LogInformation("AD authentication failed for {uername}", username);
                return null;
            }

            var claims = user.ToClaims();
            return new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public List<User> GetUsers()
        {
            return db.Users.OrderBy(u => u.FirstName).ThenBy(u => u.LastName).ToList();
        }

        public User GetUser(string username)
        {
            return db.Users.Where(u => u.Username.ToUpper() == username.ToUpper()).SingleOrDefault();
        }

        public void AddUser(User user)
        {
            db.Users.Add(user);
        }

        public List<User> SearchByPrefix(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) return new List<User>();

            prefix = prefix.ToUpper();
            return db.Users.Where(u =>
                u.FirstName.ToUpper().StartsWith(prefix) ||
                u.LastName.ToUpper().StartsWith(prefix) ||
                u.Username.ToUpper().StartsWith(prefix) ||
                u.Email.ToUpper().StartsWith(prefix) ||
                (u.FirstName + " " + u.LastName).ToUpper().StartsWith(prefix)
            ).ToList();
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}
