using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Healthy_Haven.Controllers
{
    public class EnrollmentController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAmazonSimpleNotificationService _snsClient;


        public EnrollmentController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IAmazonSimpleNotificationService snsClient)
        {
            _db = db;
            _userManager = userManager;
            _snsClient = snsClient;
        }



        [Authorize]
        [HttpPost]
        public IActionResult EnrollCourse(int courseId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var courseEnrolled = _db.CoursesEnrolled.FirstOrDefault(l => l.course_id == courseId && l.user_id == userId);

            if (courseEnrolled == null)
            {
                var newCourseEnrolled = new CoursesEnrolled
                {
                    user_id = userId,
                    course_id = courseId,
                };

                _db.CoursesEnrolled.Add(newCourseEnrolled);
                _db.SaveChanges();

                var user = _userManager.GetUserAsync(User).Result;
                string message = $"{{ \"message\": \"{user.Email}, you have successfully Enrolled into the following courseID: {courseId}.\" }}";
                string snsTopicArn = "arn:aws:sns:us-east-1:712338159638:Lambda";

                _snsClient.PublishAsync(new PublishRequest
                {
                    Message = message,
                    TopicArn = snsTopicArn
                });
            }
            else
            {
                return RedirectToAction("CourseDetails", "Course", new { Id = courseId });
            }

            return RedirectToAction("CourseDetails", "Course", new { Id = courseId });
        }

        [HttpPost]
        public IActionResult UnEnrollCourse(int courseId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var courseEnrolled = _db.CoursesEnrolled.FirstOrDefault(x => x.course_id == courseId && x.user_id == userId);

            if (courseEnrolled != null)
            {
                _db.CoursesEnrolled.Remove(courseEnrolled);
                _db.SaveChanges();
            }
            else
            {
                return RedirectToAction("CourseDetails", "Course", new { Id = courseId });
            }

            return RedirectToAction("CourseDetails", "Course", new { Id = courseId });
        }
    }
}
