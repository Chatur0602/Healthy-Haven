using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpPost]
        public IActionResult LikeComment(int commentId, int forumId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var existingLike = _db.CommentLikes.FirstOrDefault(l => l.CommentId == commentId && l.UserId == userId);

            if (existingLike == null)
            {
                var newCommentLike = new CommentLikeModel
                {
                    UserId = userId,
                    CommentId = commentId,
                };

                _db.CommentLikes.Add(newCommentLike);
                _db.SaveChanges();
            }
            else
            {
                return RedirectToAction("ViewForum", "Forum", new { Id = forumId });
            }

            return RedirectToAction("ViewForum", "Forum", new { Id = forumId });
        }

        [Authorize]
        [HttpPost]
        public IActionResult LikeForum(int forumId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var existingLike = _db.ForumLikes.FirstOrDefault(l => l.ForumId == forumId && l.UserId == userId);

            if (existingLike == null)
            {
                var newForumLike = new ForumLikeModel
                {
                    UserId = userId,
                    ForumId = forumId,
                };

                _db.ForumLikes.Add(newForumLike);
                _db.SaveChanges();
            }
            else
            {
                return RedirectToAction("ViewForum", "Forum", new { Id = forumId });
            }

            return RedirectToAction("ViewForum", "Forum", new { Id = forumId });
        }

        [HttpPost]
        public IActionResult UnlikeForum(int forumId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
          
            var existingLike = _db.ForumLikes.FirstOrDefault(x => x.ForumId == forumId && x.UserId == userId);

            if (existingLike != null)
            {
                _db.ForumLikes.Remove(existingLike);
                _db.SaveChanges();
            }
            else
            {
                return RedirectToAction("ViewForum", "Forum", new { Id = forumId });
            }

            return RedirectToAction("ViewForum", "Forum", new { Id = forumId });
        }

        [HttpPost]
        public IActionResult UnlikeComment(int commentId, int forumId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
         
            var existingLike = _db.CommentLikes.FirstOrDefault(x => x.CommentId == commentId && x.UserId == userId);

            if (existingLike != null)
            {
                _db.CommentLikes.Remove(existingLike);
                _db.SaveChanges();
            }
            else
            {
                return RedirectToAction("ViewForum", "Forum", new { Id = forumId });
            }

            return RedirectToAction("ViewForum", "Forum", new { Id = forumId });
        }
    }
}
