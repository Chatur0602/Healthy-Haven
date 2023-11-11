using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Healthy_Haven.Models
{
    public class ForumModel
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }
        
        public string Description { get; set; }

        public string User_Id { get; set; }

        public DateTime Created_At { get; set; }

    }
}
