namespace Healthy_Haven.Models
{
    public class QuizResultViewModel
    {
        public int QuizId { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public Dictionary<int, int> QuestionResponses { get; set; }
    }
}
