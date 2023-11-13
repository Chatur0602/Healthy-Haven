using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Healthy_Haven.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CommentController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public IActionResult AddComment(int forumId, string commentText)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var newComment = new CommentModel
            {
                CommentText = commentText,
                ForumId = forumId,
                UserId = userId,
                CreatedAt = DateTime.Now
            };

            _db.Comments.Add(newComment);
            _db.SaveChanges();

            return RedirectToAction("ViewForum", "Forum", new { Id = forumId });
        }

        [HttpPost]
        public IActionResult DeleteComment(int commentId, int forumId)
        {
            var commentToDelete = _db.Comments.FirstOrDefault(c => c.Id == commentId);
            

            if (commentToDelete == null)
            {
                return NotFound();
            }
            var commentLikes = _db.CommentLikes.Where(x => x.CommentId == commentId).ToList();

            _db.CommentLikes.RemoveRange(commentLikes);
            _db.Comments.Remove(commentToDelete);
            _db.SaveChanges();

            return RedirectToAction("ViewForum", "Forum", new { Id = forumId });
        }
    }
}
