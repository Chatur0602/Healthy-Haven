using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
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
        private readonly IAmazonSimpleNotificationService _snsClient;


        public QuizController(ILogger<QuizController> logger, ApplicationDbContext db, IAmazonSimpleNotificationService snsClient)
        {
            _db = db;
            _logger = logger;
            _snsClient = snsClient;

        }

        [Authorize(Roles = "Admin, Moderator,Instructor")]
        public IActionResult InstructorModeratorQuizManagement()
        {
            List<QuizzesModel> quizzes = new List<QuizzesModel>();
            quizzes = _db.Quizzes.ToList();
            quizzes.Reverse();
            return View(quizzes);
        }

        public IActionResult CreateQuiz()
        {
            var coursesList = _db.Courses.ToList();  // Replace _db.Courses with your actual DbSet for courses

            var quizCoursesModel = new QuizCoursesModel
            {
                Quizzes = new QuizzesModel(), // we fill this in the view
                CoursesList = coursesList
            };

            return View(quizCoursesModel);
        }


        [HttpPost]
        public IActionResult CreateQuiz(QuizCoursesModel details)
        {
            var coursesList = _db.Courses.ToList();
            var quizCoursesModel = new QuizCoursesModel
            {
                Quizzes = new QuizzesModel(), // in case it dont work we can reload this way
                CoursesList = coursesList
            };

            
            _db.Quizzes.Add(details.Quizzes);
            _db.SaveChanges();

            string message = $"A new Quiz has been created, check it out and Stay Updated";
            string subject = "Quiz Creation";
            string snsTopicArn = "arn:aws:sns:us-east-1:712338159638:SNSExampleSample";

            _snsClient.PublishAsync(new PublishRequest
            {
                Message = message,
                Subject = subject,
                TopicArn = snsTopicArn
            });


            return RedirectToAction("CreateQuestion", new { quizId = details.Quizzes.Id });
            
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
            var coursesList = _db.Courses.ToList();

            var quizCoursesModel = new QuizCoursesModel
            {
                Quizzes = quizDetails,
                CoursesList = coursesList
            };

            return View(quizCoursesModel);
        }

        [HttpPost]
        public IActionResult EditQuizDetails(QuizCoursesModel quizCoursesModel)
        {
            
                var existingQuiz = _db.Quizzes.Find(quizCoursesModel.Quizzes.Id);

                if (existingQuiz == null)
                {
                    return NotFound();
                }

                existingQuiz.Title = quizCoursesModel.Quizzes.Title;
                existingQuiz.Description = quizCoursesModel.Quizzes.Description;
                existingQuiz.Date = quizCoursesModel.Quizzes.Date;
                existingQuiz.CourseId = quizCoursesModel.Quizzes.CourseId;

                _db.SaveChanges();

                return RedirectToAction("InstructorModeratorQuizManagement");
            
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
                question.QuestionText = question.QuestionText;

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

        public IActionResult QuizRender(int quizId)
        {
            var quiz = _db.Quizzes.Find(quizId);
            return View(quiz);
        }

        [HttpPost]
        public IActionResult QuizResult(int quizId, Dictionary<int, int> questionResponses)
        {
            var questions = _db.Questions.Where(q => q.QuizId == quizId).ToList();
            int correctAnswers = 0;

            foreach (var question in questions)
            {
                var selectedOptionId = questionResponses.ContainsKey(question.Id) ? questionResponses[question.Id] : -1;

                var correctOption = _db.Options.FirstOrDefault(o => o.QuestionId == question.Id && o.IsCorrect);

                if (correctOption != null && correctOption.Id == selectedOptionId)
                {
                    correctAnswers++;
                }
            }

            var viewModel = new QuizResultViewModel
            {
                QuizId = quizId,
                CorrectAnswers = correctAnswers,
                TotalQuestions = questions.Count,
                QuestionResponses = questionResponses
            };

            return View("QuizResult", viewModel);
        }


    }
}