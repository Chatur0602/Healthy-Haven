using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Healthy_Haven.Models
{
    public class CoursesModel
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "Please enter the course name.")]
        public string? name { get; set; }

        [Required(ErrorMessage = "Please enter the course description.")]
        public string? description { get; set; }

        [Required(ErrorMessage = "Please enter the course date.")]
        public DateTime course_date { get; set; }

        [Required(ErrorMessage = "Please enter the credit hours.")]
        public decimal credit_hours { get; set; }

        [Required(ErrorMessage = "Please enter the valid instructor id.")]
        public string? instructor_id { get; set; }

        [ForeignKey("instructor_id")]
        public ApplicationUser Instructor { get; set; }
        public string? photo {  get; set; }
    }
}
