using System.ComponentModel.DataAnnotations;

namespace Document_Manager.Web.Models
{
    public class UploadDocumentViewModel
    {
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
