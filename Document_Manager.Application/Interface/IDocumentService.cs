using Document_Manager.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Document_Manager.Application.Interface
{
    public interface IDocumentService
    {
        Task UploadAsync(UploadDocumentDto dto);
        Task DeleteAsync(Guid documentId);
        Task<List<DocumentDto>> GetUserDocumentsAsync(Guid userId);
    }
}
