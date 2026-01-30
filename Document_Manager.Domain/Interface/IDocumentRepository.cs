using Document_Manager.Domain.Entities;

namespace Document_Manager.Domain.Interface
{
    public interface IDocumentRepository
    {
        Task AddAsync(Document document);
        Task DeleteAsync(Guid documentId);
        Task<List<Document>> GetByUserAsync(Guid userId);
        Task<Document?> GetByIdAsync(Guid id);
    }
}
