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
            // Retrieve the question based on questionId
            var question = new QuestionsModel
            {
                QuizId = quizId,
                Id = questionId // Assuming you set the Id property for the question
            };

            // Pass the question to the view
            return View(question);
        }

        /*
         * backtrack in case saveOptions doesn;t work
        [HttpPost]
        public IActionResult AddOptions(int quizId, int questionId, [FromBody] List<OptionsModel> options)
        {
            try
            {
                if (options != null && options.Count >= 2 && options.Count <= 4)
                {
                    // Ensure that only one option is selected as correct
                    if (options.Count(o => o.IsCorrect) != 1)
                    {
                        return BadRequest("Please select exactly one answer as correct.");
                    }

                    // Set the QuestionId for all options
                    foreach (var option in options)
                    {
                        option.QuestionId = questionId;
                    }

                    // Add options to the database
                    _db.Options.AddRange(options);
                    _db.SaveChanges();

                    // Return success
                    return Ok("Options added successfully.");
                }

                // Handle the case where there are validation errors or an incorrect number of options
                return BadRequest("Invalid number of options.");
            }
            catch (Exception ex)
            {
                // Log the exception, handle accordingly
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
        */
        [HttpPost]
        public IActionResult SaveOptions([FromBody] List<OptionsModel> options)
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

        [HttpPost]
        public IActionResult SaveAndNewQuestion([FromBody] List<OptionsModel> options, int quizId)
        {
            if (options != null && options.Count >= 2 && options.Count <= 4)
            {
                // Ensure that only one option is selected as correct
                if (options.Count(o => o.IsCorrect) != 1)
                {
                    // Handle the case where no correct option is selected
                    return BadRequest("Please select exactly one answer as correct.");
                }

                // Add options to the database
                _db.Options.AddRange(options);
                _db.SaveChanges();

                // Get the newly created question ID
                int questionId = options.FirstOrDefault()?.QuestionId ?? 0;

                if (questionId != 0)
                {
                    // Update the options with the correct QuestionId
                    foreach (var option in options)
                    {
                        option.QuestionId = questionId;
                    }

                    // Save the changes
                    _db.SaveChanges();
                }

                // Return a success status and redirect to CreateQuestion
                return CreateQuestion(quizId);
            }

            // Handle the case where there are validation errors or an incorrect number of options
            return BadRequest("Invalid number of options.");
        }

    }
}