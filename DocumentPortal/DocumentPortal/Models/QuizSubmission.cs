using System;
using System.Collections.Generic;

namespace DocumentPortal.Models
{
    public class QuizSubmission
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public int UserId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }

        public Quiz? Quiz { get; set; }
        public User? User { get; set; }
        public ICollection<QuizSubmissionAnswer> Answers { get; set; } = new List<QuizSubmissionAnswer>();
    }
}
