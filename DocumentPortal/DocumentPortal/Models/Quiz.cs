using System.Collections.Generic;

namespace DocumentPortal.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        public Section? Section { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<QuizSubmission> Submissions { get; set; } = new List<QuizSubmission>();
    }
}
