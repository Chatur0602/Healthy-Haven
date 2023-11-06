using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Healthy_Haven.Models
{
    public class CoursesModel
    {
        [Key]
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CourseName { get; set; }
        public string CourseDescription { get; set; }

        public int InstructorId { get; set; }
    }
}
