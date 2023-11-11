using System.ComponentModel.DataAnnotations;

namespace Healthy_Haven.Models
{
    public class CommentModel
    {
        [Key]
        public int Id { get; set; }

        public string CommentText { get; set; }
        public DateTime CreatedAt { get; set; }

        public int ForumId { get; set; }

        public string UserId { get; set; }

    }
}
