namespace Document_Manager.Domain.Entities
{
    public class Document
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public Guid UserId { get; set; }

        protected Document() { }

        public Document(string fileName, string filePath, Guid userId)
        {
            Id = Guid.NewGuid();
            FileName = fileName;
            FilePath = filePath;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
