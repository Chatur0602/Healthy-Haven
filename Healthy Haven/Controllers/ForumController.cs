using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Healthy_Haven.Controllers
{
    [Authorize(Roles = "Admin,Moderator")]
    public class ForumController : Controller
    {
        private readonly ILogger<ForumController> _logger;
        private readonly ApplicationDbContext _db;
        
        public ForumController(ILogger<ForumController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult ForumManagement()
        {
            List<ForumModel> forums = new List<ForumModel>();
            forums = _db.Forums.ToList();

            return View(forums);
        }

        public IActionResult DeleteForum(int? Id) {

            var forumDetails = _db.Forums.Find(Id);
            if (forumDetails == null)
            {
                return NotFound();
            }
            return View(forumDetails);        
        }

        [HttpPost]
        public IActionResult DeleteForumPost(int? Id)
        {
            var forumDetails = _db.Forums.Find(Id);
            if (forumDetails == null)
            {
                return NotFound();
            }

            _db.Forums.Remove(forumDetails);
            _db.SaveChanges();

            return RedirectToAction("ForumManagement");
        }

    }
}
