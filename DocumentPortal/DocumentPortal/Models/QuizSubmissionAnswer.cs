namespace DocumentPortal.Models
{
    public class QuizSubmissionAnswer
    {
        public int Id { get; set; }
        public int QuizSubmissionId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerOptionId { get; set; }

        public QuizSubmission? QuizSubmission { get; set; }
        public Question? Question { get; set; }
        public AnswerOption? AnswerOption { get; set; }
    }
}
