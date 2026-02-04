using Document_Manager.Application.DTOs;
using Document_Manager.Application.Interface;
using Document_Manager.Domain.Entities;
using Document_Manager.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Document_Manager.Web.Controllers
{
    
    public class DocumentsController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<Users> _userManager;

        public DocumentsController(
            IDocumentService documentService,
            IWebHostEnvironment env,
            UserManager<Users> userManager)
        {
            _documentService = documentService;
            _env = env;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userIdString = _userManager.GetUserId(User);

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var documents = await _documentService.GetUserDocumentsAsync(userId)
                            ?? new List<DocumentDto>();

           return View(documents);
        }

            //public async Task<IActionResult> Index()
            //{
            //    var userIdString = _userManager.GetUserId(User);

            //    if (!Guid.TryParse(userIdString, out var userId))
            //    {
            //        return Unauthorized();
            //    }

            //    var documents = await _documentService.GetUserDocumentsAsync(userId)
            //                    ?? new List<DocumentDto>();

            //    return View(documents);
            //}


            //public async Task<IActionResult> Index()
            //{
            //    var userId = _userManager.GetUserId(User);
            //    var documents = await _documentService.GetUserDocumentsAsync(Guid.Parse(userId!));
            //    return View(documents);
            //}

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
            //to the user
            var userId = Guid.Parse(_userManager.GetUserId(User));


            using var stream = new FileStream(fullPath, FileMode.Create);
            await model.File.CopyToAsync(stream);

            var dto = new UploadDocumentDto
            {
                FileName = model.File.FileName,
                FilePath = $"/uploads/documents/{fileName}",
                UserId = userId,
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