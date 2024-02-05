using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Healthy_Haven.Views.Course;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using Amazon.SimpleNotificationService.Model;
using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Authorization;


namespace Healthy_Haven.Controllers
{
    public class CourseController : Controller
    {
        private readonly ILogger<CourseController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAmazonSimpleNotificationService _snsClient;


        public CourseController(ILogger<CourseController> logger, ApplicationDbContext db, UserManager<ApplicationUser> userManager, IAmazonSimpleNotificationService snsClient)
        {
            _db = db;
            _logger = logger;
            _userManager = userManager;
            _snsClient = snsClient;
        }

        public IActionResult UserCourse(string searchTerm, string sortBy)
        {
            var courses = _db.Courses.ToList();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                courses = courses.Where(f => f.name.Contains(searchTerm) || f.description.Contains(searchTerm)).ToList();
            }

            switch (sortBy)
            {
                case "newToOld":
                    courses = courses.OrderByDescending(f => f.course_date).ToList();
                    break;
                case "oldToNew":
                    courses = courses.OrderBy(f => f.course_date).ToList();
                    break;
                case "likesLeastToMost":
                    courses = courses.OrderBy(f => _db.CoursesEnrolled.Count(x => x.course_id == f.id)).ToList();
                    break;
                case "likesMostToLeast":
                    courses = courses.OrderByDescending(f => _db.CoursesEnrolled.Count(x => x.course_id == f.id)).ToList();
                    break;

                default:
                    courses = courses.OrderByDescending(f => f.course_date).ToList();
                    break;
            }

            // Pass the sorted and filtered courses to the view
            return View(courses);
        }

        public IActionResult CourseDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseDetails = _db.Courses.Find(id);

            if (courseDetails == null)
            {
                return NotFound();
            }

            return View(courseDetails);
        }

        [Authorize(Roles = "Admin,Moderator,Instructor")]
        public IActionResult Course()
        {
            List<CoursesModel> Courses = new List<CoursesModel>();
            Courses = _db.Courses.ToList();
            return View(Courses);
        }

        [Authorize(Roles = "Admin,Moderator,Instructor")]
        public IActionResult CreateCourse()
        {
            var coursedetails = new CoursesModel();
            return View(coursedetails);
        }


        [Authorize(Roles = "Admin,Moderator,Instructor")]
        [HttpPost]
        public async Task<IActionResult> Create(CoursesModel coursedetails, List<IFormFile> files)
        {
            var user = await _userManager.GetUserAsync(User);

            try
            {
                Debug.WriteLine("COURSE NAME= " + coursedetails.name + "COURSE DATE= " + coursedetails.course_date);
                coursedetails.instructor_id = user.Id;
                coursedetails.course_date = DateTime.Now;
                _db.Courses.Add(coursedetails);
                _db.SaveChanges();

                int maxFileCount = 5;
                int fileCount = 1;
                int course_Id = coursedetails.id;

                IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

                var accessKeyId = configuration["AWSCredentials:AccessKeyId"];
                var secretAccessKey = configuration["AWSCredentials:SecretAccessKey"];
                var sessionToken = configuration["AWSCredentials:SessionToken"];

                using (var s3Client = new AmazonS3Client(accessKeyId, secretAccessKey, sessionToken, Amazon.RegionEndpoint.USEast1))
                {
                    foreach (var file in files)
                    {
                        if (file != null && file.Length > 0)
                        {
                            if (fileCount > maxFileCount)
                            {
                                ViewBag.Error = "Exceeded the maximum allowed files (5).";
                                DelFunction(course_Id);
                                break;
                            }

                            using (var memorystream = new MemoryStream())
                            {
                                file.CopyTo(memorystream);
                                var key = "CourseImages/" + file.FileName;
                                var request = new TransferUtilityUploadRequest
                                {
                                    InputStream = memorystream,
                                    Key = key,
                                    BucketName = "healthyheaven",
                                    ContentType = file.ContentType,
                                };

                                var transferUtility = new TransferUtility(s3Client);

                                Debug.WriteLine($"Uploading image to S3. Key: {key}");

                                await transferUtility.UploadAsync(request);

                                Debug.WriteLine($"Image uploaded successfully.");

                                CourseImages courseImages = new CourseImages
                                {
                                    image_path = file.FileName,
                                    course_id = course_Id
                                };

                                _db.CourseImages.Add(courseImages);
                                _db.SaveChanges();
                            }
                        }

                        fileCount++;
                    }
                }

                var baseUrl = "http://healthy-haven.us-east-1.elasticbeanstalk.com";
                var forumUrl = $"{baseUrl}/Course/CourseDetails/{coursedetails.id}";

                string message = $"A new course {coursedetails.name} has been created, check it out at {forumUrl}";
                string subject = "Course Creation";
                string snsTopicArn = "arn:aws:sns:us-east-1:712338159638:SNSExampleSample";

                await _snsClient.PublishAsync(new PublishRequest
                {
                    Message = message,
                    Subject = subject,
                    TopicArn = snsTopicArn
                });



                return RedirectToAction("CreateModule", new { courseId = coursedetails.id });
            }
            catch (Exception ex)
            {
                // Handle exceptions (log or provide user feedback)
                Debug.WriteLine($"An error occurred: {ex.Message}");
                return RedirectToAction("ErrorPage"); // Redirect to an error page or handle appropriately
            }
        }
        [Authorize(Roles = "Admin,Moderator,Instructor")]
        public IActionResult DelFunction(int? id)
        {
            var courseDel = _db.Courses.Find(id);
            var courseImage = _db.CourseImages.Where(x => x.course_id == id).ToList();

            _db.CourseImages.RemoveRange(courseImage);
            _db.Courses.Remove(courseDel);
            _db.SaveChanges();

            return View();
        }
        [Authorize(Roles = "Admin,Moderator,Instructor")]
        public IActionResult Edit(int? id)
        {
            var coursedetails = _db.Courses.Find(id);
            if (coursedetails == null)
            {
                return NotFound();
            }
            return View("EditCourse", coursedetails);
        }
        [Authorize(Roles = "Admin,Moderator,Instructor")]
        [HttpPost]
        public IActionResult Edit(CoursesModel coursedetails)
        {
            _db.Courses.Update(coursedetails);
            _db.SaveChanges();
            return RedirectToAction("Course");
        }
        [Authorize(Roles = "Admin,Moderator,Instructor")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coursedetails = _db.Courses.Find(id);
            if (coursedetails == null)
            {
                return NotFound();
            }
            var courseImages = _db.CourseImages.Where(img => img.course_id == id).ToList();
            ViewBag.CourseImages = courseImages;

            return View("DeleteCourse", coursedetails);
        }
        [Authorize(Roles = "Admin,Moderator,Instructor")]
        [HttpPost]
        public async Task<IActionResult> DeleteCourse(int? id, List<string> selectedFileNames)
        {
            var coursedetails = _db.Courses.Find(id);
            var courseModules = _db.Modules.Where(x => x.course_id == id).ToList();
            var courseEnrolled = _db.CoursesEnrolled.Where(x => x.course_id == id).ToList();
            var courseQuizzes = _db.Quizzes.Where(x => x.CourseId == id).ToList();

            if (id == null)
            {
                return NotFound();
            }

            if (coursedetails == null)
            {
                return NotFound();
            }
            IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            var accessKeyId = configuration["AWSCredentials:AccessKeyId"];
            var secretAccessKey = configuration["AWSCredentials:SecretAccessKey"];
            var sessionToken = configuration["AWSCredentials:SessionToken"];

            using (var s3Client = new AmazonS3Client(accessKeyId, secretAccessKey, sessionToken, Amazon.RegionEndpoint.USEast1))
            {
                foreach(var fileName in selectedFileNames)
                {
                    System.Diagnostics.Debug.WriteLine("filename" + fileName);

                    var folderPath = "CourseImages/";
                    var key = folderPath + fileName;

                    await s3Client.DeleteObjectAsync(new DeleteObjectRequest()
                    {
                        BucketName = "healthyheaven",
                        Key = key,
                    });

                    var courseImage = _db.CourseImages.FirstOrDefault(x => x.course_id == id);
                    _db.CourseImages.Remove(courseImage);
                    _db.SaveChanges();
                }
            }

            _db.Modules.RemoveRange(courseModules);
            _db.CoursesEnrolled.RemoveRange(courseEnrolled);
            _db.Quizzes.RemoveRange(courseQuizzes);

            foreach (var module in courseModules)
            {
                var moduleChapters = _db.Chapters.Where(x => x.module_id == module.id).ToList();
                _db.Chapters.RemoveRange(moduleChapters);
            }

            foreach (var quiz in courseQuizzes)
            {
                var quizQuestions = _db.Questions.Where(x => x.QuizId == quiz.Id).ToList();
                _db.Questions.RemoveRange(quizQuestions);

                foreach (var question in quizQuestions)
                {
                    var questionOptions = _db.Options.Where(x => x.QuestionId == question.Id).ToList();
                    _db.Options.RemoveRange(questionOptions);
                }
            }

            _db.Courses.Remove(coursedetails);
            _db.SaveChanges();

            return RedirectToAction("Course");
        }

        [Authorize(Roles = "Admin,Moderator,Instructor")]
        public IActionResult ViewModules(int courseId)
        {
            var Modules = _db.Modules.Where(q => q.course_id == courseId).ToList();
            return View(Modules);
        }
        [Authorize(Roles = "Admin,Moderator,Instructor")]
        public IActionResult CreateModule(int courseId)
        {
            var course = _db.Courses.Find(courseId);

            if (course == null)
            {
                // Handle the case where the quiz is not found
                return NotFound();
            }

            ModulesModel module = new ModulesModel();
            module.course_id = courseId;
            ViewData["Module"] = module;

            return View(module);
        }

        [Authorize(Roles = "Admin,Moderator,Instructor")]
        [HttpPost]
        public IActionResult CreateMod(ModulesModel moduledetails)
        {

                _db.Modules.Add(moduledetails);
                _db.SaveChanges();

                return RedirectToAction("CreateChapter", new { moduleId = moduledetails.id });

        }

        public IActionResult EditModule(int? id)
        {
            var moduledetails = _db.Modules.Find(id);
            if (moduledetails == null)
            {
                return NotFound();
            }
            return View("EditModule", moduledetails);
        }

        [HttpPost]
        public IActionResult EditMod(ModulesModel moduledetails)
        {
            _db.Modules.Update(moduledetails);
            _db.SaveChanges();
            return RedirectToAction("ViewModules", new { courseid = moduledetails.course_id });
        }

        public IActionResult DeleteModule(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var moduledetails = _db.Modules.Find(id);
            if (moduledetails == null)
            {
                return NotFound();
            }

            return View("DeleteModule", moduledetails);
        }

        [HttpPost]
        public IActionResult DeleteMod(int? id)
        {
            var moduledetails = _db.Modules.Find(id);
            var moduleChapters = _db.Chapters.Where(x => x.module_id == id).ToList();

            if (id == null)
            {
                return NotFound();
            }

            if (moduledetails == null)
            {
                return NotFound();
            }

            _db.Chapters.RemoveRange(moduleChapters);
            _db.Modules.Remove(moduledetails);

            _db.SaveChanges();

            return RedirectToAction("ViewModules", new { courseid = moduledetails.course_id });
        }

        public IActionResult ViewChapters(int moduleId)
        {
            var Modules = _db.Chapters.Where(q => q.module_id == moduleId).ToList();
            return View(Modules);
        }

        public IActionResult CreateChapter(int moduleId)
        {
            var module = _db.Modules.Find(moduleId);

            if (module == null)
            {
                // Handle the case where the quiz is not found
                return NotFound();
            }

            ChapterModel chapter = new ChapterModel();
            chapter.module_id = moduleId;
            ViewData["Chapter"] = chapter;

            return View(chapter);
        }


        [HttpPost]
        public IActionResult CreateChap(ChapterModel chapterDetails)
        {

            _db.Chapters.Add(chapterDetails);
            _db.SaveChanges();

            return RedirectToAction("ViewChapters", new { moduleId = chapterDetails.module_id });

        }

        public IActionResult EditChapter(int? id)
        {
            var chapterdetails = _db.Chapters.Find(id);
            if (chapterdetails == null)
            {
                return NotFound();
            }
            return View("EditChapter", chapterdetails);
        }

        [HttpPost]
        public IActionResult EditChap(ChapterModel chapterdetails)
        {
            _db.Chapters.Update(chapterdetails);
            _db.SaveChanges();
            return RedirectToAction("ViewChapters", new { moduleId = chapterdetails.module_id });
        }

        public IActionResult DeleteChapter(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapterdetails = _db.Chapters.Find(id);
            if (chapterdetails == null)
            {
                return NotFound();
            }

            return View("DeleteChapter", chapterdetails);
        }

        [HttpPost]
        public IActionResult DeleteChap(int? id)
        {
            var chapterdetails = _db.Chapters.Find(id);

            if (id == null)
            {
                return NotFound();
            }

            if (chapterdetails == null)
            {
                return NotFound();
            }
            _db.Chapters.Remove(chapterdetails);

            _db.SaveChanges();

            return RedirectToAction("ViewChapters", new { moduleId = chapterdetails.module_id });
        }
    }
}

