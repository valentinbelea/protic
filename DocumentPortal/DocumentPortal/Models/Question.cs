using System.Collections.Generic;

namespace DocumentPortal.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsMultipleChoice { get; set; }

        public Quiz? Quiz { get; set; }
        public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
    }
}
