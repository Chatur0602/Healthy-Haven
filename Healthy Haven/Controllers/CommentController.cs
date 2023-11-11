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
            // Get the current user
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Create a new comment
            var newComment = new CommentModel
            {
                CommentText = commentText,
                ForumId = forumId,
                UserId = userId,
                CreatedAt = DateTime.Now
            };

            // Add the new comment to the database
            _db.Comments.Add(newComment);
            _db.SaveChanges();

            // Redirect back to the forum page after adding the comment
            return RedirectToAction("ViewForum", "Forum", new { Id = forumId });
        }
    }
}
