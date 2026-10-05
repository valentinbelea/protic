using System.Collections.Generic;

namespace DocumentPortal.Models
{
    public class Section
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }

        public Menu? Menu { get; set; }
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    }
}
