using Fair.Services;
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
    }
}
