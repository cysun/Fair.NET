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
            return View();
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

        [HttpGet("Searches/{searchId}/Documents/{documentId}/Revisions/Add")]
        public IActionResult AddRevision(int searchId, int documentId)
        {
            ViewBag.Search = searchService.GetSearch(searchId);
            ViewBag.Document = documentService.GetDocument(documentId);
            return View();
        }

        [HttpPost("Searches/{searchId}/Documents/{documentId}/Revisions/Add")]
        public IActionResult AddRevision(int searchId, int documentId, string notes, IFormFile uploadedFile)
        {
            var document = documentService.GetDocument(documentId);
            var user = Models.User.PrincipalToUser(User);
            var revision = new Revision
            {
                DocumentId = documentId,
                Number = document.LatestRevision.Number + 1,
                AuthorId = user.UserId,
                Notes = notes,
                File = Models.File.FromUploadedFile(uploadedFile, user.UserId)
            };
            document.Revisions.Add(revision);
            document.LatestRevision = revision;
            documentService.SaveChanges();

            logger.LogInformation("{username} added revision {revisionNumber} to document {documentId}",
                user.Username, revision.Number, documentId);

            return Redirect($"../../View/{documentId}");
        }
    }
}
