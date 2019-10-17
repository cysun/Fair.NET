using System;
using System.Collections.Generic;
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

        public IActionResult View(int searchId, int applicationId)
        {
            ViewBag.Search = searchService.GetSearch(searchId);
            return View(applicationService.GetApplication(applicationId));
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

            return Redirect($"EditDocuments/{application.Id}");
        }

        [HttpGet]
        public IActionResult Edit(int searchId, int applicationId)
        {
            ViewBag.Search = searchService.GetSearch(searchId);
            return View(applicationService.GetApplication(applicationId));
        }

        [HttpPost]
        public IActionResult Edit(int applicationId, Application update)
        {
            var application = applicationService.GetApplication(applicationId);
            application.CopyFrom(update);
            applicationService.SaveChanges();

            logger.LogInformation("{username} updated application {applicationId}", User.FindFirst(ClaimTypes.Name).Value, applicationId);

            return Redirect($"../EditDocuments/{applicationId}");
        }

        [HttpGet]
        public IActionResult Evaluate(int searchId, int applicationId)
        {
            ViewBag.Search = searchService.GetSearch(searchId);
            return View(applicationService.GetApplication(applicationId));
        }

        [HttpPost]
        public IActionResult Evaluate(int applicationId, Application update)
        {
            var application = applicationService.GetApplication(applicationId);
            application.HaveMinimumQualifications = update.HaveMinimumQualifications;
            application.HavePreferredQualifications = update.HavePreferredQualifications;
            application.IsAdvancedToPhoneInterview = update.IsAdvancedToPhoneInterview;
            application.IsAdvancedToCampusInterview = update.IsAdvancedToCampusInterview;
            application.Notes = update.Notes;
            application.DateEvaluated = DateTime.Now;
            applicationService.SaveChanges();

            logger.LogInformation("{username} updated evaluation for {applicationId}", User.FindFirst(ClaimTypes.Name).Value, applicationId);

            return Redirect($"../View/{applicationId}");
        }

        [HttpGet]
        public IActionResult EditDocuments(int searchId, int applicationId)
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

            logger.LogInformation("{username} uploaded document {fileId} for application {applicationId}",
                user.Username, application.Documents[index].FileId, application.Id);

            return Ok();
        }

        [HttpGet("Searches/{searchId}/Applications/{applicationId}/Documents/View/{index}")]
        public IActionResult ViewDocument(int applicationId, int index)
        {
            var application = applicationService.GetApplication(applicationId);
            var file = fileService.GetFile(application.Documents[index].FileId);
            return File(file.OpenReadStream(), file.ContentType);
        }

        [HttpGet("Searches/{searchId}/Applications/{applicationId}/Documents/Download/{index}")]
        public IActionResult DownloadDocument(int applicationId, int index)
        {
            var application = applicationService.GetApplication(applicationId);
            var file = fileService.GetFile(application.Documents[index].FileId);
            return File(file.OpenReadStream(), file.ContentType, file.Name);
        }

        [HttpGet]
        public IActionResult EditReferences(int searchId, int applicationId)
        {
            ViewBag.Search = searchService.GetSearch(searchId);
            return View(applicationService.GetApplication(applicationId));
        }

        [HttpPost]
        public IActionResult EditReferences(int applicationId, List<ApplicationReference> references)
        {
            var application = applicationService.GetApplication(applicationId);
            application.References = references;
            applicationService.SaveChanges();

            logger.LogInformation("{username} updated the references of application {applicationId}",
                User.FindFirst(ClaimTypes.Name).Value, applicationId);

            return Redirect($"../View/{applicationId}");
        }

        [HttpGet("Searches/{searchId}/Applications/{applicationId}/ReferenceReports")]
        public IActionResult ReferenceReports(int searchId, int applicationId)
        {
            ViewBag.Search = searchService.GetSearch(searchId);
            return View(applicationService.GetApplication(applicationId));
        }

        [HttpPost("Searches/{searchId}/Applications/{applicationId}/ReferenceReports/{index}")]
        public IActionResult UploadReferenceReport(int applicationId, int index, [FromForm(Name = "file")] IFormFile uploadedFile)
        {
            var application = applicationService.GetApplication(applicationId);
            var user = Models.User.PrincipalToUser(User);

            application.References[index].Report = Models.File.FromUploadedFile(uploadedFile, user.Id);
            applicationService.SaveChanges();

            logger.LogInformation("{username} uploaded reference report {fileId} for application {applicationId}",
                user.Username, application.References[index].ReportId, application.Id);

            return Ok();
        }

        [HttpGet("Searches/{searchId}/Applications/{applicationId}/ReferenceReports/Download/{index}")]
        public IActionResult DownloadReferenceReport(int applicationId, int index)
        {
            var application = applicationService.GetApplication(applicationId);
            var file = fileService.GetFile(application.References[index].ReportId);
            return File(file.OpenReadStream(), file.ContentType, file.Name);
        }
    }
}
