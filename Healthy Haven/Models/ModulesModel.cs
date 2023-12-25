using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Healthy_Haven.Models
{
    public class ModulesModel
    {
        public int id { get; set; }
       
        public string module { get; set; }
        public int course_id { get; set; }

    }
}
