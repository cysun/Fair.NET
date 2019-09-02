using Fair.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fair.Controllers
{
    public class FilesController : Controller
    {
        private readonly FileService fileService;

        public FilesController(FileService fileService)
        {
            this.fileService = fileService;
        }

        public IActionResult View(int id)
        {
            var file = fileService.GetFile(id);
            return File(file.OpenReadStream(), file.ContentType);
        }

        public IActionResult Download(int id)
        {
            var file = fileService.GetFile(id);
            return File(file.OpenReadStream(), file.ContentType, file.Name);
        }
    }
}
