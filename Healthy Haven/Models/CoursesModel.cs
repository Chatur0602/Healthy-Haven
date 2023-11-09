using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Healthy_Haven.Models
{
    public class CoursesModel
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string course_duration { get; set; }
        public string credit_hours { get; set; }


        public int instructor_id { get; set; }
    }
}
