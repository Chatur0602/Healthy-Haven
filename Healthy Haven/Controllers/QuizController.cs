using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Healthy_Haven.Controllers
{
    public class QuizController : Controller
    {
        private readonly ILogger<QuizController> _logger;
        private readonly ApplicationDbContext _db;

        public QuizController(ILogger<QuizController> logger, ApplicationDbContext db)
        {
            _db = db;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,Moderator,Instructor")]
        public IActionResult QuizManagement()
        {
            List<QuizDBEntity> questions = new List<QuizDBEntity>();
            questions = _db.Questions.ToList();
            return View(questions);
        }

        public IActionResult Create() 
        {
            return View();
        }

        [HttpPost]
        public  IActionResult Create(QuizDBEntity questionDetails)
        {
            if (ModelState.IsValid)
            {
                _db.Questions.Add(questionDetails);
                _db.SaveChanges();
                return RedirectToAction("QuizBuilder");
            }
            return View();
        }
    }
}
