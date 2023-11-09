using System.ComponentModel.DataAnnotations;

namespace Healthy_Haven.Models
{
    public class ForumLikeModel
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public int ForumId { get; set; }
    }
}
