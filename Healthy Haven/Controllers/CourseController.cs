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
            var coursedetails = new CoursesModel();
            return View(coursedetails);
        }



        [HttpPost]
        public async Task<IActionResult> Create(CoursesModel coursedetails, List<IFormFile> files)
        {
            try
            {
                Debug.WriteLine("COURSE NAME= " + coursedetails.name + "COURSE DATE= " + coursedetails.course_date);
                _db.Courses.Add(coursedetails);
                _db.SaveChanges();

                int course_Id = coursedetails.id;

                using (var amazonS3client = new AmazonS3Client("ASIA55H4D3RU3YGBSRC2", "b9NK4j9Q1Yr06QA6pGtwSM3o27h4JqOXoby+mbV+", "FwoGZXIvYXdzEGgaDBkMuLt8g08U5gPcmCK8AVVCxej8nXNSwFsaB07hFdFhgb2B+b+bXB2hKP7i5VSlUrnOS/IrdwSMmLXuLsW/LZKUc1r/dViFnptCHvL0orWYtKi7w/GPF6Ik6fWu5SsJTErRuFiuAqBdYry/0vdcvbYidn0xz0Xatl1aaLn0BeUzvaxORNIRUNDmTtwNAhvUaqjn29VmCJ4MiYKIL9W3ZqilUdXjMq9K32xaTDiF9rF/SGRtvPBDxAybhvcCSAkVDRKrpfI4if//OQMPKJ2a3KoGMi34qE6qpuveWUmolNzGHL6RCp7cGa61r/99fFE12NIbnVTlqLWLIjaXESbM5yU=", RegionEndpoint.USEast1))
                {
                foreach (var file in files)
                {
                    using (var memorystream = new MemoryStream())
                    {
                        file.CopyTo(memorystream);
                        var key = "CourseImages/" + file.FileName;
                        var request = new TransferUtilityUploadRequest
                        {
                            InputStream = memorystream,
                            Key = key,
                            BucketName = "healthyhavens3",
                            ContentType = file.ContentType,
                        };

                        var transferUtility = new TransferUtility(amazonS3client);
                        Debug.WriteLine($"Uploading image to S3. Key: {key}");
                        await transferUtility.UploadAsync(request);
                        Debug.WriteLine($"Image uploaded successfully.");

                            CourseImages courseImages = new CourseImages();
                            courseImages.image_path = file.FileName;
                            courseImages.course_id = course_Id;
                    }
                }
            }

            return RedirectToAction("Course");
            }
            catch (Exception ex)
            {
                // Handle exceptions (log or provide user feedback)
                Debug.WriteLine($"An error occurred: {ex.Message}");
                return RedirectToAction("ErrorPage"); // Redirect to an error page or handle appropriately
            }
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

