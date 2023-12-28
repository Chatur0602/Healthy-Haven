using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Healthy_Haven.Controllers
{
    public class EnrollmentController : Controller
    {


        private readonly ApplicationDbContext _db;

        public EnrollmentController(ApplicationDbContext db)
        {
            _db = db;
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
