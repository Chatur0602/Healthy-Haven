using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
            quizzes.Reverse();
            return View(quizzes);
        }

        /*
        public IActionResult QuizBuilder(int quizId)
        {
            // Create a new empty question object
            QuestionsModel question = new QuestionsModel();

            // Set the quiz ID
            question.QuizId = quizId;

            return View(question);
        }
        */

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

            return View();
        }


        public IActionResult DeleteQuiz(int quizId)
        {
            var quiz = _db.Quizzes.Find(quizId);

            if (quiz == null)
            {
                return NotFound();
            }

            // Retrieve associated questions
            var questions = _db.Questions.Where(q => q.QuizId == quizId).ToList();

            foreach (var question in questions)
            {
                // Retrieve associated options
                var options = _db.Options.Where(o => o.QuestionId == question.Id).ToList();

                // Remove options
                _db.Options.RemoveRange(options);

                // Remove question
                _db.Questions.Remove(question);
            }

            // delete quiz and save
            _db.Quizzes.Remove(quiz);
            _db.SaveChanges();

            return RedirectToAction("InstructorModeratorQuizManagement");
        }

        public IActionResult EditQuiz (int quizId)
        {
            var quizDetails = _db.Quizzes.Find(quizId);

            return View(quizDetails);
        }

        [HttpPost]
        public IActionResult EditQuizDetails (QuizzesModel quizDetails)
        {
            if (ModelState.IsValid)
            {
                var existingQuiz = _db.Quizzes.Find(quizDetails.Id);

                if (existingQuiz == null)
                {
                    return NotFound();
                }

                
                existingQuiz.Title = quizDetails.Title;
                existingQuiz.Description = quizDetails.Description;
                existingQuiz.Date = quizDetails.Date;

                // Save the changes to the database
                _db.SaveChanges();

                return RedirectToAction("InstructorModeratorQuizManagement");
            }

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
                //Automatically assign number to question e.g 1. something
                int quizCount = _db.Questions.Count(q => q.QuizId == quizId);
                String qNo = (quizCount+1) + ". ";
                question.QuestionText = qNo + question.QuestionText;

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
                    return BadRequest("Please select one answer as correct.");
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