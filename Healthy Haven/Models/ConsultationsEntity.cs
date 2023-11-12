using System.ComponentModel.DataAnnotations;

namespace Healthy_Haven.Models
{


    public class ConsultationsEntity
    {
        [Key]
        public int id { get; set; }
        public string description { get; set; }
        public int student_id { get; set; }
        public int instructor_id { get; set; }
        public DateTime date { get; set; }

    }
}
