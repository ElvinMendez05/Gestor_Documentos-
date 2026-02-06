using Document_Manager.Application.DTOs;
using Document_Manager.Application.Interface;
using Document_Manager.Domain.Entities;
using Document_Manager.Domain.Interface;

namespace Document_Manager.Application.Service
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _repository;

        public DocumentService(IDocumentRepository repository)
        {
            _repository = repository;
        }

        public async Task DeleteAsync(Guid documentId)
        {
            await _repository.DeleteAsync(documentId);
        }

        public async Task UploadAsync(UploadDocumentDto dto)
        {
            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(dto.FileName))
                throw new ArgumentException("File name is required");

            if (!dto.FileName.EndsWith(".pdf"))
                throw new ArgumentException("Only PDF files are allowed");

            var document = new Document(
                dto.FileName,
                dto.FilePath,
                dto.UserId
            );

            await _repository.AddAsync(document);
        }

        public async Task<List<DocumentDto>> GetUserDocumentsAsync(Guid userId)
        {
            var documents = await _repository.GetByUserAsync(userId);

            return documents.Select(d => new DocumentDto
            {
                Id = d.Id,
                FileName = d.FileName,
                FilePath = d.FilePath,
                CreatedAt = d.CreatedAt
            }).ToList();
        }
    }
}
