using Healthy_Haven.Controllers;
using System.ComponentModel.DataAnnotations;

namespace Healthy_Haven.Models
{


    public class ConsultationsEntity
    {
        [Key]
        public int id { get; set; }
        public string description { get; set; }
        public string student_id { get; set; }
        public string instructor_id { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Consultation Date")]
        [Required(ErrorMessage = "The Consultation Date field is required.")]
        [FutureDate(ErrorMessage = "Please select a future date.")]
        public DateTime date { get; set; }

    }
}
