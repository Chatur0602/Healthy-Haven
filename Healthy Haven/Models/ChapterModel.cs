using System.ComponentModel.DataAnnotations.Schema;

namespace Healthy_Haven.Models
{
    public class ChapterModel
    {

        public int id { get; set; }
        public required string name { get; set; }
        public required string content { get; set; }
        public int module_id { get; set; }


    }
}
