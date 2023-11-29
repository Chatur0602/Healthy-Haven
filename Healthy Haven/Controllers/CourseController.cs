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

                int maxFileCount = 5;
                int fileCount = 1;
                int course_Id = coursedetails.id;

                using (var amazonS3client = new AmazonS3Client("ASIA55H4D3RUQGOO6EQB", "Jv9ebiMrVz2ZQ7ORXa8m2N4pTeD1hHtmq9IQ3Gym", "FwoGZXIvYXdzEG0aDHoXKHtl3q7HnsoQNSK8AUlMPWKfzwaw4jqfYITulPqOrhEKs0ZDdir7qW7yRXvXCESwUV5I4xAxM9BE7YmA+ZArdSvso1S9QE9MpIg0dDamEKmRdp2MwWP6/nu7ZrWprUwP/n64ykAfJq6K4u6yQsu8NO9af3ZGlwZrc5nkDR9puPAx9UXcSc+wlB2LBe/eac2j//Qpei4cNfPyg56/4iFI402dbABavnXFrCFvgrM8LGUyq5UGV/PR68+0tIHrZtdpdi6B5yotxscPKICk3aoGMi1vz0AbsFU2br2oWMimqtDdbs/2idJbXslx3/E0lsHw8cynO3SRnk59eg2SxRo=", RegionEndpoint.USEast1))
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
                                    BucketName = "healthyhavens3",
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

            using (var amazons3client = new AmazonS3Client("ASIA55H4D3RUQGOO6EQB", "Jv9ebiMrVz2ZQ7ORXa8m2N4pTeD1hHtmq9IQ3Gym", "FwoGZXIvYXdzEG0aDHoXKHtl3q7HnsoQNSK8AUlMPWKfzwaw4jqfYITulPqOrhEKs0ZDdir7qW7yRXvXCESwUV5I4xAxM9BE7YmA+ZArdSvso1S9QE9MpIg0dDamEKmRdp2MwWP6/nu7ZrWprUwP/n64ykAfJq6K4u6yQsu8NO9af3ZGlwZrc5nkDR9puPAx9UXcSc+wlB2LBe/eac2j//Qpei4cNfPyg56/4iFI402dbABavnXFrCFvgrM8LGUyq5UGV/PR68+0tIHrZtdpdi6B5yotxscPKICk3aoGMi1vz0AbsFU2br2oWMimqtDdbs/2idJbXslx3/E0lsHw8cynO3SRnk59eg2SxRo=", RegionEndpoint.USEast1))
            {
                foreach(var fileName in selectedFileNames)
                {
                    System.Diagnostics.Debug.WriteLine("filename" + fileName);

                    var folderPath = "CourseImages/";
                    var key = folderPath + fileName;

                    await amazons3client.DeleteObjectAsync(new DeleteObjectRequest()
                    {
                        BucketName = "healthyhavens3",
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

