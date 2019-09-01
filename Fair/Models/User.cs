using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace Fair.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        [NotMapped]
        public string Name => $"{FirstName} {LastName}";

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        public bool IsAdmin { get; set; } = false;

        public bool IsSysAdmin { get; set; } = false;

        public List<Claim> ToClaims()
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, UserId.ToString()),
                new Claim(ClaimTypes.Name, Username),
                new Claim(ClaimTypes.GivenName, FirstName),
                new Claim(ClaimTypes.Surname, LastName),
                new Claim(ClaimTypes.Email, Email),
                new Claim("IsAdmin", (IsAdmin || IsSysAdmin).ToString()),
                new Claim("IsSysAdmin", IsSysAdmin.ToString())
            };
        }

        public static User PrincipalToUser(ClaimsPrincipal principal)
        {
            var user = new User();
            user.UserId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier).Value);
            user.Username = principal.FindFirst(ClaimTypes.Name).Value;
            user.FirstName = principal.FindFirst(ClaimTypes.GivenName).Value;
            user.LastName = principal.FindFirst(ClaimTypes.Surname).Value;
            user.Email = principal.FindFirst(ClaimTypes.Email).Value;
            user.IsAdmin = principal.HasClaim("IsAdmin", true.ToString());
            user.IsSysAdmin = principal.HasClaim("IsSysAdmin", true.ToString());
            return user;
        }
    }
}
