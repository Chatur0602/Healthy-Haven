//using Healthy_Haven.Views.Course;
using System.ComponentModel.DataAnnotations;

namespace Healthy_Haven.Models
{
    public class QuizzesModel
    {
        [Key]
        public int Id { get; set; }

        
        
        public string Title { get; set; }

        
        
        public string Description { get; set; }

        
        public int CourseId { get; set; }

        
        public string UserId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        // Navigation properties
        //public CourseModel Course { get; set; }
        //public ApplicationUser Author { get; set; }

        // Collection navigation property for questions
        //public ICollection<QuestionsModel> Questions { get; set; }
    }
}
