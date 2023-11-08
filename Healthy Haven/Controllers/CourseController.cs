using Healthy_Haven.Data;
using Healthy_Haven.Models;
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
	}
}
