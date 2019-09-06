using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fair.Models;
using Fair.Security;
using Fair.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fair.Controllers
{
    public class SearchesController : Controller
    {
        private readonly SearchService searchService;
        private readonly DepartmentService departmentService;
        private readonly IAuthorizationService authService;
        private readonly ILogger<SearchesController> logger;

        public SearchesController(SearchService searchService, DepartmentService departmentService,
            IAuthorizationService authService, ILogger<SearchesController> logger)
        {
            this.searchService = searchService;
            this.departmentService = departmentService;
            this.authService = authService;
            this.logger = logger;
        }

        public IActionResult List()
        {
            return View(searchService.GetSearches(Models.User.PrincipalToUser(User)));
        }

        public IActionResult View(int id)
        {
            return View(searchService.GetSearch(id));
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var search = new Search();
            var authResult = await authService.AuthorizeAsync(User, search, "CanWriteSearch"); // auth resource cannot be null
            if (!authResult.Succeeded)
                return Forbid();

            if (User.HasClaim(claim => claim.Type == FairClaims.DepartmentChair.ToString()))
            {
                int departmentId = int.Parse(User.FindFirst(FairClaims.DepartmentChair.ToString()).Value);
                search.DepartmentId = departmentId;
                search.Department = departmentService.GetDepartment(departmentId);
            }
            return View(search);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Search search, List<int> committeeMemberIds)
        {
            var authResult = await authService.AuthorizeAsync(User, search, "CanWriteSearch");
            if (!authResult.Succeeded)
                return Forbid();

            searchService.AddSearch(search);
            searchService.SaveChanges();

            foreach (var committeeMemberId in committeeMemberIds)
                search.CommitteeMembers.Add(new CommitteeMember(search.SearchId, committeeMemberId));
            searchService.SaveChanges();

            logger.LogInformation("{username} created search {searchId}", User.FindFirst(ClaimTypes.Name).Value, search.SearchId);

            return RedirectToAction(nameof(View), new { id = search.SearchId });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View(searchService.GetSearch(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Search update, List<int> committeeMemberIds)
        {
            var search = searchService.GetSearch(id);
            search.DepartmentId = update.DepartmentId;
            search.Position = update.Position;
            search.DepartmentChairId = update.DepartmentChairId;
            search.CommitteeChairId = update.CommitteeChairId;
            search.CommitteeMembers.RemoveAll(m => !committeeMemberIds.Contains(m.UserId));
            committeeMemberIds.RemoveAll(memberId => search.CommitteeMembers.Select(m => m.UserId).Contains(memberId));
            search.CommitteeMembers.AddRange(committeeMemberIds.Select(memberId => new CommitteeMember(search.SearchId, memberId)));

            var authResult = await authService.AuthorizeAsync(User, search, "CanWriteSearch");
            if (!authResult.Succeeded)
                return Forbid();

            searchService.SaveChanges();

            logger.LogInformation("{username} updated search {searchId}", User.FindFirst(ClaimTypes.Name).Value, search.SearchId);

            return RedirectToAction(nameof(View), new { id });
        }
    }
}
