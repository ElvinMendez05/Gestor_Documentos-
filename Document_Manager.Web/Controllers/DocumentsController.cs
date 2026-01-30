using Document_Manager.Application.DTOs;
using Document_Manager.Application.Interface;
using Document_Manager.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Document_Manager.Web.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly IWebHostEnvironment _env;

        public DocumentsController(
            IDocumentService documentService,
            IWebHostEnvironment env)
        {
            _documentService = documentService;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse("11111111-1111-1111-1111-111111111111"); // mock user
            var documents = await _documentService.GetUserDocumentsAsync(userId);
            return View(documents);
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(UploadDocumentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!model.File.FileName.EndsWith(".pdf"))
            {
                ModelState.AddModelError("", "Solo se permiten archivos PDF");
                return View(model);
            }

            var uploadsPath = Path.Combine(
                _env.WebRootPath,
                "uploads",
                "documents"
            );

            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}.pdf";
            var fullPath = Path.Combine(uploadsPath, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await model.File.CopyToAsync(stream);

            var dto = new UploadDocumentDto
            {
                FileName = model.File.FileName,
                FilePath = $"/uploads/documents/{fileName}",
                UserId = Guid.Parse("11111111-1111-1111-1111-111111111111")
            };

            await _documentService.UploadAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _documentService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}