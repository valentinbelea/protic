using System.Collections.Generic;
using DocumentPortal.Models;

namespace DocumentPortal.ViewModels
{
    public class TakeQuizViewModel
    {
        public Quiz Quiz { get; set; } = null!;
        // Dictionary mapping QuestionId to a list of selected AnswerOptionIds
        public Dictionary<int, List<int>> Answers { get; set; } = new Dictionary<int, List<int>>();
        public QuizSubmission? PreviousSubmission { get; set; }
    }

    public class QuizResultViewModel
    {
        public Quiz Quiz { get; set; } = null!;
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public double ScorePercentage => TotalQuestions > 0 ? ((double)CorrectAnswers / TotalQuestions) * 100 : 0;
    }
}
