using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Healthy_Haven.Models
{
    public class QuizDBEntity
    {
        [Key]
        public int question_id { get; set; }
        
        public int category_id { get; set; }
        [DisplayName("Question name")]
        public string question_name { get; set; }
        [DisplayName("is Active")]
        public bool isActive { get; set; }
        [DisplayName("Multiple options")]
        public bool isMultiple { get; set; }

    }
}
