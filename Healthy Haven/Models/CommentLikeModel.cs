using System.ComponentModel.DataAnnotations;

namespace Healthy_Haven.Models
{
    public class CommentLikeModel
    {

        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public int CommentId { get; set; }

    }
}
