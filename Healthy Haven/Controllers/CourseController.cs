using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Healthy_Haven.Views.Course;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Healthy_Haven.Controllers
{
	public class CourseController : Controller
	{
		private readonly ILogger<CourseController> _logger;
		private readonly ApplicationDbContext _db;

		public CourseController(ILogger<CourseController> logger, ApplicationDbContext db)
		{
			_db = db;
			_logger = logger;
		}


		public IActionResult Course()
		{
			List<CoursesModel> Courses = new List<CoursesModel>();
			Courses = _db.Courses.ToList();

			return View(Courses);
		}

        
        public IActionResult CreateCourse()
        {
            return View();
        }

        [HttpPost]
		public IActionResult Create(CoursesModel coursedetails)
		{
            //Debug.WriteLine("COURSE NAME= " + coursedetails.name + "COURSE DATE= " + coursedetails.course_date);
            _db.Courses.Add(coursedetails);
			_db.SaveChanges();

            return RedirectToAction("Course");
		}

		public IActionResult Edit(int? id)
		{
			var coursedetails = _db.Courses.Find(id);
			if (coursedetails == null)
			{
				return NotFound();
			}
			return View("CreateCourse", coursedetails);
		}

		[HttpPost]
        public IActionResult Edit(CoursesModel coursedetails)
        {
			if (ModelState.IsValid)
			{
				_db.Courses.Update(coursedetails);
			}
            return View("CreateCourse", coursedetails);
        }

        public IActionResult Delete(int? id)
        {
            var coursedetails = _db.Courses.Find(id);
            if (coursedetails == null)
            {
                return NotFound();
            }
            return View("CreateCourse", coursedetails);
        }

   //     [HttpPost]
   //     public IActionResult Delete(int? id)
   //     {
   //         var coursedetails = _db.Courses.Find(id);
   //         if (coursedetails == null)
   //         {
   //             return NotFound();
   //         }
			//_db.Courses.Remove(coursedetails);
			//_db.SaveChanges();
   //         return RedirectToAction("Course");
   //     }
    }
}
