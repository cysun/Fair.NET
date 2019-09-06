using System.Collections.Generic;
using System.Linq;
using Fair.Models;

namespace Fair.Services
{
    public class UserService
    {
        private readonly AppDbContext db;

        public UserService(AppDbContext db)
        {
            this.db = db;
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
