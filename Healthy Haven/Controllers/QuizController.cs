using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

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

        [Authorize(Roles = "Moderator,Instructor")]
        public IActionResult InstructorModeratorQuizManagement()
        {
            List<QuizzesModel> quizzes = new List<QuizzesModel>();
            quizzes = _db.Quizzes.ToList();
            return View(quizzes);
        }

        public IActionResult QuizBuilder(int quizId)
        {
            CategoryViewModel objCategoryViewModel = new CategoryViewModel();
            
            return View();
        }

        /* Video implementation. For reference
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
        */
        public IActionResult CreateQuiz()
        {
            //ViewBag.Courses = new SelectList(_db.Courses, "Id", "Name"); // Assuming you have a DbSet<CoursesModel> in your DbContext

            return View();
        }

        [HttpPost]
        public IActionResult CreateQuiz(QuizzesModel quizDetails)
        {
            if (ModelState.IsValid)
            {
                _db.Quizzes.Add(quizDetails);
                _db.SaveChanges();

                return RedirectToAction("QuizBuilder", new { quizId = quizDetails.Id });
            }

            // If validation fails, return to the same view
            return View();
        }



    }
}
