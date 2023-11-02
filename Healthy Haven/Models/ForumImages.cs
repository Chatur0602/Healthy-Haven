using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Healthy_Haven.Models
{
    public class ForumImages
    {
        [Key]
        public int Id { get; set; }

        public string Image_Path { get; set; }
     
        public int Forum_Id {  get; set; }

    }
}
