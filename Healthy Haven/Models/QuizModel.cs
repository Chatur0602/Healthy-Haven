using System.ComponentModel.DataAnnotations;

namespace Healthy_Haven.Models
{
    public class QuizModel
    {
        [Key]
        public int quizID { get; set; }

        public string name { get; set; }
        public string description { get; set; }
        public string authorID { get; set; }
        public string date { get; set; }
        public string quiz_data { get; set; }
    }
}
