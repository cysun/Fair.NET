using System.Linq;
using System.Security.Claims;
using Fair.Models;
using Fair.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fair.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly FileService fileService;
        private readonly SearchService searchService;
        private readonly ApplicationService applicationService;
        private readonly ApplicationTemplateService applicationTemplateService;
        private readonly ILogger<ApplicationsController> logger;

        public ApplicationsController(FileService fileService, SearchService searchService, ApplicationService applicationService,
            ApplicationTemplateService applicationTemplateService, ILogger<ApplicationsController> logger)
        {
            this.fileService = fileService;
            this.searchService = searchService;
            this.applicationService = applicationService;
            this.applicationTemplateService = applicationTemplateService;
            this.logger = logger;
        }

        public IActionResult List(int searchId)
        {
            var search = searchService.GetSearch(searchId);
            search.Applications = search.Applications.OrderBy(a => a.DateSubmitted).ToList();
            return View(search);
        }

        [HttpGet]
        public IActionResult Add(int searchId)
        {
            var applicationTemplate = applicationTemplateService.GetSearchApplicationTemplate(searchId);
            ViewBag.Search = searchService.GetSearch(searchId);
            return View(new Application().AddFieldsFromTemplate(applicationTemplate));
        }

        [HttpPost]
        public IActionResult Add(int searchId, Application application)
        {
            var applicationTemplate = applicationTemplateService.GetSearchApplicationTemplate(searchId);
            application.AddFieldsFromTemplate(applicationTemplate);
            applicationService.AddApplication(application);
            applicationService.SaveChanges();

            logger.LogInformation("{username} created application {applicationId}", User.FindFirst(ClaimTypes.Name).Value, application.Id);

            return Redirect($"EditApplicationDocuments/{application.Id}");
        }

        [HttpGet]
        public IActionResult EditApplication(int searchId, int applicationId)
        {
            ViewBag.Search = searchService.GetSearch(searchId);
            return View(applicationService.GetApplication(applicationId));
        }

        [HttpGet]
        public IActionResult EditApplicationDocuments(int searchId, int applicationId)
        {
            ViewBag.Search = searchService.GetSearch(searchId);
            return View(applicationService.GetApplication(applicationId));
        }

        [HttpPost("Searches/{searchId}/Applications/{applicationId}/Documents/{index}")]
        public IActionResult UploadDocument(int searchId, int applicationId, int index, [FromForm(Name = "file")] IFormFile uploadedFile)
        {
            var application = applicationService.GetApplication(applicationId);
            var user = Models.User.PrincipalToUser(User);

            application.Documents[index].File = Models.File.FromUploadedFile(uploadedFile, user.Id);
            applicationService.SaveChanges();

            logger.LogInformation("{username} uploaded document {documentName} for application {applicationId}",
                user.Username, application.Documents[index].Name, application.Id);

            return Ok();
        }

        [HttpGet("Searches/{searchId}/Applications/{applicationId}/Documents/View/{index}")]
        public IActionResult ViewDocument(int searchId, int applicationId, int index)
        {
            var application = applicationService.GetApplication(applicationId);
            var file = fileService.GetFile(application.Documents[index].FileId);
            return File(file.OpenReadStream(), file.ContentType);
        }

        [HttpGet("Searches/{searchId}/Applications/{applicationId}/Documents/Download/{index}")]
        public IActionResult DownloadDocument(int searchId, int applicationId, int index)
        {
            var application = applicationService.GetApplication(applicationId);
            var file = fileService.GetFile(application.Documents[index].FileId);
            return File(file.OpenReadStream(), file.ContentType, file.Name);
        }
    }
}
