using System;
using System.Linq;
using System.Security.Claims;
using Fair.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;

namespace Fair.Services
{
    public class AuthenticationService
    {
        private readonly IADService adService;
        private readonly UserService userService;
        private readonly SearchService searchService;
        private readonly DepartmentService departmentService;
        private readonly ILogger<AuthenticationService> logger;

        public AuthenticationService(IADService adService, UserService userService, SearchService searchService,
            DepartmentService departmentService, ILogger<AuthenticationService> logger)
        {
            this.adService = adService;
            this.userService = userService;
            this.searchService = searchService;
            this.departmentService = departmentService;
            this.logger = logger;
        }

        public ClaimsIdentity Authenticate(string username, string password)
        {
            var user = userService.GetUser(username);
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

            var departments = departmentService.GetDepartments(user);
            foreach (var department in departments)
                claims.Add(new Claim(FairClaims.DepartmentChair.ToString(), department.DepartmentId.ToString()));

            var searches = searchService.GetSearches(user);
            foreach (var search in searches)
            {
                if (search.DepartmentChairId == user.UserId)
                    claims.Add(new Claim(FairClaims.DepartmentChair.ToString(), search.SearchId.ToString()));
                else if (search.CommitteeChairId == user.UserId)
                    claims.Add(new Claim(FairClaims.SearchDepartmentChair.ToString(), search.SearchId.ToString()));
                else if (search.CommitteeMembers.Select(m => m.UserId).Contains(user.UserId))
                    claims.Add(new Claim(FairClaims.SearchCommitteeMember.ToString(), search.SearchId.ToString()));
            }

            return new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        }

    }
}
