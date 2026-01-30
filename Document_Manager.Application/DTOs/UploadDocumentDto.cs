using System;
using System.Collections.Generic;
using System.Text;

namespace Document_Manager.Application.DTOs
{
    public class UploadDocumentDto
    {
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}
