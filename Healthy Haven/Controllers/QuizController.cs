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
            // Create a new empty question object
            QuestionsModel question = new QuestionsModel();

            // Set the quiz ID
            question.QuizId = quizId;

            return View(question);
        }


        public IActionResult CreateQuiz()
        {
            return View();
        }


        [HttpPost]
        public IActionResult CreateQuiz(QuizzesModel quizDetails)
        {
            if (ModelState.IsValid)
            {
                _db.Quizzes.Add(quizDetails);
                _db.SaveChanges();

                // Redirect to the quiz builder page with the newly created quiz ID
                return RedirectToAction("CreateQuestion", new { quizId = quizDetails.Id });
            }

            // If validation fails, return to the same view
            return View();
        }

        public IActionResult CreateQuestion(int quizId)
        {
            // Retrieve the quiz based on quizId
            var quiz = _db.Quizzes.Find(quizId);

            if (quiz == null)
            {
                // Handle the case where the quiz is not found
                return NotFound();
            }

            // Create a new empty question object
            QuestionsModel question = new QuestionsModel();

            // Set the quiz ID
            question.QuizId = quizId;

            // Pass the quiz to the view
            ViewData["Quiz"] = quiz;

            return View(question);
        }

        


        [HttpPost]
        public IActionResult AddQuestion(int quizId, QuestionsModel question)
        {
            if (ModelState.IsValid)
            {
                // Add the question to the database
                _db.Questions.Add(question);
                _db.SaveChanges();

                // Redirect to the page for adding options with the newly created question ID
                return RedirectToAction("AddOptions", new { quizId = quizId, questionId = question.Id });
            }

            // Validation failed, return to the create question page
            return RedirectToAction("CreateQuestion", new { quizId = quizId });
        }

        public IActionResult AddOptions(int quizId, int questionId)
        {
            // Retrieve the question based on questionId from the database
            var question = _db.Questions
                              .Where(q => q.Id == questionId && q.QuizId == quizId)
                              .FirstOrDefault();

            if (question == null)
            {
                // Handle the case where the question is not found
                return NotFound();
            }

            // Pass the question to the view
            return View(question);
        }

        
        [HttpPost]
        public IActionResult SaveOptions([FromBody] List<OptionsModel> options)  //saves options on submit in Add Options
        {
            if (options != null && options.Count >= 2 && options.Count <= 4)
            {
                // Ensure that only one option is selected as correct
                if (options.Count(o => o.IsCorrect) != 1)
                {
                    // Handle the case where no correct option is selected
                    return BadRequest("Please select exactly one answer as correct.");
                }

                // Set the QuestionId for all options
                foreach (var option in options)
                {
                    //option.QuestionId = questionId;
                }

                // Add options to the database
                _db.Options.AddRange(options);
                _db.SaveChanges();

                // Return a success status
                return Ok();
            }

            // Handle the case where there are validation errors or an incorrect number of options
            return BadRequest("Invalid number of options.");
        }

        

    }
}