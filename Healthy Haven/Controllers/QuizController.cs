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
            List<QuestionsModel> questions = new List<QuestionsModel>();
            questions = _db.Questions.ToList();
            return View(questions);
        }

        public IActionResult QuizBuilder()
        {
            CategoryViewModel objCategoryViewModel = new CategoryViewModel();
            
            return View();
        }

        public IActionResult CreateQuiz() 
        {
            return View();
        }

        [HttpPost]
        public  IActionResult CreateQuiz(QuestionsModel questionDetails)
        {
            //modify later -- should link to the builder tab as it says tho
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
