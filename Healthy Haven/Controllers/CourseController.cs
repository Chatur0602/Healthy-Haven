using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Healthy_Haven.Views.Course;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

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

        public IActionResult UserCourse()
        {
            List<CoursesModel> Courses = new List<CoursesModel>();
            Courses = _db.Courses.ToList();
            return View(Courses);
        }
        public IActionResult CourseDetails(int? id)
		{
            var coursedetails = _db.Courses.Find(id);
            if (coursedetails == null)
            {
                return NotFound();
            }
            return View("UserCourse", new List<CoursesModel> { coursedetails });
        }
		public IActionResult Course()
		{
			List<CoursesModel> Courses = new List<CoursesModel>();
			Courses = _db.Courses.ToList();

			return View(Courses);
		}

        
        public IActionResult CreateCourse()
        {
            List<CoursesModel> files = new List<CoursesModel>();
            files = _db.Courses.ToList();
            return View(files);
        }

        [HttpPost]
		public async Task<IActionResult> Create(CoursesModel coursedetails, List<IFormFile> files)
		{
            using (var amazonS3client = new AmazonS3Client("ASIA55H4D3RU5PVRPW4C", "2INDCqTHlj9I0Ps8X2zeZYm/l9l+RTUh3bG825YV", "FwoGZXIvYXdzELz//////////wEaDP93jwo8t0aem23RwyK8AVJ4brFIvJ3Wtp/5Derbf4U4WmkgQujFQlgljVemYiRo1OP7Nh6r0vle8JwrfXcPjFKxqUMphY+nUNQxlyl+WmRJjP9Cf0CLJL+eXvvFdDzzrGp/ykcPn6dD2L2l2vpoIEyB4RvzRkS/PMUj5OFSB/bOxtdupuQyBzIgmycOTyVXcWzjBicH/C+yaeRRWrlDK8rMD4qjKEIPXTVu+w6FyMFL/z3gzyAeou7WtpJ1NiLrE1j6NZ7q5U6rDy+cKKq4tqoGMi39vKCSxdWRH5GgdCEQpi0fEyx+jbX5ofKFyChV2TR5oWyh0MrXP0OhYeYn2oE=", RegionEndpoint.USEast1))
            {
                foreach (var file in files)
                {
                    using (var memorystream = new MemoryStream())
                    {
                        file.CopyTo(memorystream);
                        var request = new TransferUtilityUploadRequest
                        {
                            InputStream = memorystream,
                            Key = file.FileName,
                            BucketName = "healthyhavens3",
                            ContentType = file.ContentType,
                        };

                        var transferUtility = new TransferUtility(amazonS3client);
                        await transferUtility.UploadAsync(request);
                    }
                }
            }

            Debug.WriteLine("COURSE NAME= " + coursedetails.name + "COURSE DATE= " + coursedetails.course_date);
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
			return View("EditCourse", coursedetails);
		}

		[HttpPost]
        public IActionResult Edit(CoursesModel coursedetails)
        {
			
			_db.Courses.Update(coursedetails);
			_db.SaveChanges();
            return RedirectToAction("Course");
        }

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

            return View("DeleteCourse", coursedetails);
        }

		[HttpPost]
		public IActionResult DeleteCourse(int? id)
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

            _db.Courses.Remove(coursedetails);
            _db.SaveChanges();

            return RedirectToAction("Course");
        }
	}
}
