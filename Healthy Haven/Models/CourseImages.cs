using System.ComponentModel.DataAnnotations;

namespace Healthy_Haven.Models
{
    public class CourseImages
    {
        [Key]
        public int id { get; set; }

        public int course_id { get; set; }
        public string image_path { get; set; }
    }
}
