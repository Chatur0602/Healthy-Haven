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

                int maxFileCount = 5;
                int fileCount = 1;
                int course_Id = coursedetails.id;

                using (var amazonS3client = new AmazonS3Client("ASIA2LWVJXALHUDRZVDO", "p3nm02aDr+4SLIH4jkel61bCJCrmoOlk82vZls5I", "FwoGZXIvYXdzEI///////////wEaDOX0v6WYbOuOQ+hgbiK8AVMynKu6MJSVhrV5ZgsOMv1dtK9GZVjQOYFLdmFanAfou3y62GHkxwSUYFqhbQ7eSEU2jMgoGLnatsDdU9XpzGr2q1K6wP24H2E73snbEjicgQeskI7wAZrMDVXfT6bY/T9OjntH8TST49+uLErUDZ188iZHIaQ8QO21/p8rQBGflqRbPc9gJq1oC5+Nn+eD6er3mZdopju70+rqa6uvyDHYqP95uiY+j+Q7lUD7uSeOY40BAfFGzcBlHOT0KPT8nKsGMi2Mhsv2xonmj+7bidiaBmgEGUfv2o0ehJ0vzSnM+d1+udgDwLUuuVgeruxeubM=", RegionEndpoint.USEast1))
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

                                var transferUtility = new TransferUtility(amazonS3client);

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

                return RedirectToAction("Course");
            }
            catch (Exception ex)
            {
                // Handle exceptions (log or provide user feedback)
                Debug.WriteLine($"An error occurred: {ex.Message}");
                return RedirectToAction("ErrorPage"); // Redirect to an error page or handle appropriately
            }
        }

        public IActionResult DelFunction(int? id)
        {
            var courseDel = _db.Courses.Find(id);
            var courseImage = _db.CourseImages.Where(x => x.course_id == id).ToList();

            _db.CourseImages.RemoveRange(courseImage);
            _db.Courses.Remove(courseDel);
            _db.SaveChanges();

            return View();
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
            var courseImages = _db.CourseImages.Where(img => img.course_id == id).ToList();
            ViewBag.CourseImages = courseImages;

            return View("DeleteCourse", coursedetails);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCourse(int? id, List<string> selectedFileNames)
        {
            var coursedetails = _db.Courses.Find(id);

            if (id == null)
            {
                return NotFound();
            }

            if (coursedetails == null)
            {
                return NotFound();
            }

            using (var amazons3client = new AmazonS3Client("ASIA2LWVJXALJ24ABY7U", "S0fef4bzmZwwJEkEmNeE6tLvefZueL2Pa2kOHrdv", "FwoGZXIvYXdzEIr//////////wEaDM7cef35Y8pGytHEjyK8ATTeAChswZ9YoVsiYne9JD4IqS7ZOfQ/9cWGG2rxH5DsnEPp1tKw54mRR+1IrXFct4Vj44mZwiPGMhqX2rVU7Fv8KmXYg+EXeSiXofXmhfTDQDhlCRuxxL7uGUSjCAEP2mQw8s5vk10Vyga/cLhSyTAVOUbYUNm9gbmScvRd47K5z69zDH7ONdyWrFT5HTAt1HjMfRUl4PobIY+B5ESvOqkVUY688apRN9D9HlnqOvCqGTOnYvFmg/dZAP7KKM72m6sGMi01erfYCJBRlVfIbET4HFYezAGwkZmCQFAK0s+KV362m0HQRiUTJWkR6Qc/DG8=", RegionEndpoint.USEast1))
            {
                foreach(var fileName in selectedFileNames)
                {
                    System.Diagnostics.Debug.WriteLine("filename" + fileName);

                    var folderPath = "CourseImages/";
                    var key = folderPath + fileName;

                    await amazons3client.DeleteObjectAsync(new DeleteObjectRequest()
                    {
                        BucketName = "healthyheaven",
                        Key = key,
                    });

                    var courseImage = _db.CourseImages.FirstOrDefault(x => x.course_id == id);
                    _db.CourseImages.Remove(courseImage);
                    _db.SaveChanges();
                }
            }

            _db.Courses.Remove(coursedetails);
            _db.SaveChanges();

            return RedirectToAction("Course");
        }
    }

}

