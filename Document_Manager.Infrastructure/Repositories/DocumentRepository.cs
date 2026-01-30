using Document_Manager.Domain.Entities;
using Document_Manager.Domain.Interface;
using Document_Manager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Document_Manager.Infrastructure.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AppDbContext _context;

        public DocumentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Document document)
        {
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid documentId)
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == documentId);

            if (document is null)
                return;

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }

        public async Task<Document?> GetByIdAsync(Guid id)
        {
            return await _context.Documents
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<List<Document>> GetByUserAsync(Guid userId)
        {
            return await _context.Documents
                .AsNoTracking()
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

   
    }
}
