using Fair.Models;
using Fair.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fair.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly SearchService searchService;
        private readonly DocumentService documentService;
        private readonly ILogger<DocumentsController> logger;

        public DocumentsController(SearchService searchService, DocumentService documentService, ILogger<DocumentsController> logger)
        {
            this.searchService = searchService;
            this.documentService = documentService;
            this.logger = logger;
        }

        public IActionResult List(int searchId)
        {
            ViewBag.Search = searchService.GetSearch(searchId);
            return View(documentService.GetDocuments(searchId));
        }

        public IActionResult View(int searchId, int documentId)
        {
            ViewBag.Search = searchService.GetSearch(searchId);
            return View(documentService.GetDocument(documentId));
        }

        [HttpGet]
        public IActionResult Add(int searchId)
        {
            ViewBag.Search = searchService.GetSearch(searchId);
            return View(new Document());
        }

        [HttpPost]
        public IActionResult Add(int searchId, string name, string notes, IFormFile uploadedFile)
        {
            var document = new Document
            {
                SearchId = searchId,
                Name = name
            };
            documentService.AddDocument(document);
            documentService.SaveChanges();

            var user = Models.User.PrincipalToUser(User);
            var revision = new Revision
            {
                DocumentId = document.DocumentId,
                Number = 1,
                AuthorId = user.UserId,
                Notes = notes,
                File = Models.File.FromUploadedFile(uploadedFile, user.UserId)
            };
            document.Revisions.Add(revision);
            document.LatestRevision = revision;
            documentService.SaveChanges();

            logger.LogInformation("{username} created document {documentId}", user.Username, document.DocumentId);

            return Redirect($"View/{document.DocumentId}");
        }
    }
}
