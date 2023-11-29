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

            var questions = _db.Questions.Where(q => q.QuizId == quizId).ToList();

            foreach (var question in questions)
            {
                var options = _db.Options.Where(o => o.QuestionId == question.Id).ToList();

                _db.Options.RemoveRange(options);
                _db.Questions.Remove(question);
            }

            // delete quiz and save
            _db.Quizzes.Remove(quiz);
            _db.SaveChanges();

            return RedirectToAction("InstructorModeratorQuizManagement");
        }


        public IActionResult DeleteQuestion (int  questionId)
        {
            var question = _db.Questions.Find(questionId);

            if(question == null)
            {
                return NotFound();
            }

            var options = _db.Options.Where(o => o.QuestionId == question.Id).ToList();
            _db.Options.RemoveRange(options);
            _db.Questions.Remove(question);
            _db.SaveChanges();


            return RedirectToAction("QuestionManagement", new { quizId = question.QuizId });
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

                _db.SaveChanges();

                return RedirectToAction("InstructorModeratorQuizManagement");
            }

            return View();
        }

        /*
         So, i take them to the view to edit the question
         post to method which then updates the model based on the questionId
         post method redirects to questionManagement with quizID in the question Model
         */
        public IActionResult EditQuestion( int quizId)  //can imporve but later
        {
            var question = _db.Questions.Find(quizId);
            return View(question);
        }

        [HttpPost]
        public IActionResult EditQuestion (QuestionsModel questionDetails )
        {   
            if (ModelState.IsValid)
            {
                var existingQuestions = _db.Questions.Find(questionDetails.Id);

                if (existingQuestions == null)
                {
                    return NotFound();
                }

                existingQuestions.QuestionText = questionDetails.QuestionText;
                _db.SaveChanges();

                return RedirectToAction("QuestionManagement", new { quizId = questionDetails.QuizId });
            }

            return View();

        }

        public IActionResult CreateQuestion(int quizId)
        {
            var quiz = _db.Quizzes.Find(quizId);

            if (quiz == null)
            {
                // Handle the case where the quiz is not found
                return NotFound();
            }

            QuestionsModel question = new QuestionsModel();
            question.QuizId = quizId;
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

                _db.Questions.Add(question);
                _db.SaveChanges();

                return RedirectToAction("AddOptions", new { quizId = quizId, questionId = question.Id });
            }

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
                return NotFound();
            }

            return View(question);
        }

        public IActionResult EditOptions(int questionId)
        {
            var question = _db.Questions.Find(questionId);

            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        [HttpPost]
        public IActionResult EditOptions([FromBody] List<OptionsModel> options)
        {
            if (options != null && options.Count >= 2 && options.Count <= 4)
            {
                if (options.Count(o => o.IsCorrect) != 1)
                {
                    // Handle the case where no correct option is selected
                    return BadRequest("Please select one answer as correct.");
                }


                var questionId = options[0].QuestionId;
                var existingOptions = _db.Options.Where(o => o.QuestionId == options[0].QuestionId).ToList();
                _db.Options.RemoveRange(existingOptions);
                _db.SaveChanges();

                _db.Options.AddRange(options);
                _db.SaveChanges();

                return RedirectToAction("OptionManagement", new { questionId = questionId });
            }

            return BadRequest("Invalid number of options.");
        }

        
        [HttpPost]
        public IActionResult SaveOptions([FromBody] List<OptionsModel> options)  //saves options on submit in Add Options
        {
            if (options != null && options.Count >= 2 && options.Count <= 4)
            {
                if (options.Count(o => o.IsCorrect) != 1)
                {
                    // Handle the case where no correct option is selected
                    return BadRequest("Please select one answer as correct.");
                }

                _db.Options.AddRange(options);
                _db.SaveChanges();

                return RedirectToAction("InstructorModeratorQuizManagement");
            }

            return BadRequest("Invalid number of options.");
        }


        public IActionResult QuestionManagement(int quizId)  
        {
            var questions = _db.Questions.Where(q => q.QuizId == quizId).ToList();
            return View(questions);
        }

        public IActionResult OptionManagement (int questionId)
        {
            var options = _db.Options.Where(q => q.QuestionId == questionId).ToList();
            return View(options);
        }
        

    }
}