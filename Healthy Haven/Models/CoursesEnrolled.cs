using System.ComponentModel.DataAnnotations;

namespace Healthy_Haven.Models
{
    public class CoursesEnrolled
    {
        [Key]
        public int Id { get; set; }

        public string user_id { get; set; }

        public int course_id { get; set; }
    }
}

