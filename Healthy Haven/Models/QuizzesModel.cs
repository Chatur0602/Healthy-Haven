//using Healthy_Haven.Views.Course;
using System.ComponentModel.DataAnnotations;

namespace Healthy_Haven.Models
{
    public class QuizzesModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        // Navigation properties
        //public CourseModel Course { get; set; }
        public ApplicationUser Author { get; set; }

        // Collection navigation property for questions
        public ICollection<QuestionsModel> Questions { get; set; }
    }
}
