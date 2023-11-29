using System.ComponentModel.DataAnnotations;

namespace Healthy_Haven.Models
{
    public class OptionsModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        [StringLength(500)]
        public string OptionText { get; set; }

        [Required]
        public bool IsCorrect { get; set; }

        // Navigation properties
        public QuestionsModel Question { get; set; }
    }
}
