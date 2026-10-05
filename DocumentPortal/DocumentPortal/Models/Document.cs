using System;

namespace DocumentPortal.Models
{
    public class Document
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public string? FileName { get; set; }
        public string? Description { get; set; }
        public string? Content { get; set; }
        public int Order { get; set; } = 0;
        public string? FilePath { get; set; }
        public string? ContentType { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        public Section? Section { get; set; }
    }
}
