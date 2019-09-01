using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Fair.Models;
using Fair.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fair.Controllers
{
    public class SearchesController : Controller
    {
        private readonly SearchService searchService;

        private readonly ILogger<SearchesController> logger;

        public SearchesController(SearchService searchService, ILogger<SearchesController> logger)
        {
            this.searchService = searchService;
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
        [Authorize(Policy = "IsAdmin")]
        public IActionResult Add()
        {
            return View(new Search());
        }

        [HttpPost]
        [Authorize(Policy = "IsAdmin")]
        public IActionResult Add(Search search, List<int> committeeMemberIds)
        {
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
        public IActionResult Edit(int id, Search update, List<int> committeeMemberIds)
        {
            var search = searchService.GetSearch(id);
            search.Name = update.Name;
            search.DepartmentChairId = update.DepartmentChairId;
            search.CommitteeChairId = update.CommitteeChairId;
            search.CommitteeMembers.RemoveAll(m => !committeeMemberIds.Contains(m.UserId));
            committeeMemberIds.RemoveAll(memberId => search.CommitteeMembers.Select(m => m.UserId).Contains(memberId));
            search.CommitteeMembers.AddRange(committeeMemberIds.Select(memberId => new CommitteeMember(search.SearchId, memberId)));
            searchService.SaveChanges();

            logger.LogInformation("{username} updated search {searchId}", User.FindFirst(ClaimTypes.Name).Value, search.SearchId);

            return RedirectToAction(nameof(View), new { id });
        }
    }
}
