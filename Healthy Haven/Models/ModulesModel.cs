using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Healthy_Haven.Models
{
    public class ModulesModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string content { get; set; }
        public string? course_id { get; set; }

        [ForeignKey("course_id")]
        public ApplicationUser Course { get; set; }
    }
}
