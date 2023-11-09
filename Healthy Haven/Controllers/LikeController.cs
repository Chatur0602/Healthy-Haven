using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Healthy_Haven.Controllers
{
    public class LikeController : Controller
    {

        private readonly ApplicationDbContext _db;

        public LikeController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public IActionResult LikeComment(int commentId, int forumId)
        {
            // Get the current user
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if the user has already liked this comment
            var existingLike = _db.CommentLikes.FirstOrDefault(l => l.CommentId == commentId && l.UserId == userId);

            if (existingLike == null)
            {
                // User hasn't liked this comment yet, so create a new like
                var newCommentLike = new CommentLikeModel
                {
                    UserId = userId,
                    CommentId = commentId,
                    
                    // Add any other properties you need for the like
                };

                _db.CommentLikes.Add(newCommentLike);
                _db.SaveChanges();
            }
            else
            {
                // User has already liked this comment, you may want to handle this case accordingly
                // For now, let's just redirect back to the forum page
                return RedirectToAction("ViewForum", "Forum", new { Id = forumId });
            }

            // Redirect back to the forum page after liking the comment
            return RedirectToAction("ViewForum", "Forum", new { Id = forumId });
        }

        [HttpPost]
        public IActionResult LikeForum(int forumId)
        {
            // Get the current user
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if the user has already liked this comment
            var existingLike = _db.ForumLikes.FirstOrDefault(l => l.ForumId == forumId && l.UserId == userId);

            if (existingLike == null)
            {
                // User hasn't liked this comment yet, so create a new like
                var newForumLike = new ForumLikeModel
                {
                    UserId = userId,
                    ForumId = forumId,

                    // Add any other properties you need for the like
                };

                _db.ForumLikes.Add(newForumLike);
                _db.SaveChanges();
            }
            else
            {
                // User has already liked this comment, you may want to handle this case accordingly
                // For now, let's just redirect back to the forum page
                return RedirectToAction("ViewForum", "Forum", new { Id = forumId });
            }

            // Redirect back to the forum page after liking the comment
            return RedirectToAction("ViewForum", "Forum", new { Id = forumId });
        }

    }
}
