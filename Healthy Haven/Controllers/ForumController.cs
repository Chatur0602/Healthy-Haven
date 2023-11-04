using Amazon.S3.Transfer;
using Amazon.S3;
using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Amazon;
using Microsoft.Identity.Client;
using Amazon.S3.Model;
using Microsoft.CodeAnalysis.Elfie.PDB;

namespace Healthy_Haven.Controllers
{
    public class ForumController : Controller
    {
        private readonly ILogger<ForumController> _logger;
        private readonly ApplicationDbContext _db;
        UserManager<ApplicationUser> _userManager;

        public ForumController(ILogger<ForumController> logger, ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager; 
        }

        public IActionResult ForumDashboard()
        {
            List<ForumModel> forums = new List<ForumModel>();
            forums = _db.Forums.ToList();

            return View(forums);

        }


        [HttpGet]
        public async Task<IActionResult> CreateForum()
        {
       
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateForum(ForumModel forumDetails, List<IFormFile> files)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
               
                forumDetails.User_Id = user.Id; 
                forumDetails.Created_At = DateTime.Now; 
                _db.Forums.Add(forumDetails);
                _db.SaveChanges();

                int forumId = forumDetails.Id;

                if (files != null && files.Count > 0)
                {
                    int totalSizeLimit = 5 * 1024 * 1024; // 5MB
                    int maxFileCount = 5;
                    string[] allowedImageTypes = { "image/jpeg", "image/png", "image/gif" };

                    int totalSize = 1;
                    int fileCount = 1;



          
                 using (var amazonS3client = new AmazonS3Client("ASIA55H4D3RU4A4FHPPY", "iNLGeewV9yUxA6IuNkyMOemy8AsNbjlZLZ7viMnu", "FwoGZXIvYXdzECoaDP3L8HjwKXk8UPlC1yK8ATGPPPKzE5cHLUIh9LG3lQugAslLW7vOcDBfg5jvE66KR57LtJ0c/lpL/Zh0EzKfjXTtsswSiubbR59uH7jKLIpzMk7kn+htzWuZm3uKw/vxfByMovndFw+d/lK3ybV8RV3eoW0nhL71vcDrhtLp4k9joIwSin+6n3m4T5pBWajXPdHGVpg7n+DfRNDTrDTTlbZJlOV5xIoQGzITpOSa0bHJ1OXaDdkQlrKufn7EcO1pmz6LjOdbFb0/dpwFKJCmlqoGMi3chmlUe5k0/qK3R/BpyOJvURk+gNPxMcMEH6p5vXyB9LN/polJQZBVKB+2svk=", RegionEndpoint.USEast1))
                    {
                        foreach (var file in files)
                        {
                            if (file != null && file.Length > 0)
                            {
                                if (file.Length > 5 * 1024 * 1024) // 5MB
                                {
                                    ViewBag.Error = "File size exceeds the limit (5MB).";
                                    DelFunction(forumId);
                                    break;
                                }
                                else if (totalSize > totalSizeLimit)
                                {
                                    ViewBag.Error = "Total file size exceeds the limit (5MB).";
                                    DelFunction(forumId);
                                    break;
                                }
                                else if (fileCount > maxFileCount)
                                {
                                    ViewBag.Error = "Exceeded the maximum allowed files (5).";
                                    DelFunction(forumId);
                                    break;
                                }
                                else if (!allowedImageTypes.Contains(file.ContentType))
                                {
                                    ViewBag.Error = "Invalid file type. Only image files (JPEG, PNG, GIF) are allowed.";
                                    DelFunction(forumId);
                                    break;
                                }
                                else
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

                                        ForumImages forumImages = new ForumImages();
                                        forumImages.Image_Path = file.FileName;
                                        forumImages.Forum_Id = forumId;

                                        _db.ForumImages.Add(forumImages);
                                        _db.SaveChanges();
                                    }
                                    }
                                }
                            fileCount++;
                            totalSize += totalSize;
                            }
                        }
                  
                    if (!string.IsNullOrEmpty(ViewBag.Error))
                    {
                        
                        return View(forumDetails);
                    }
                }
            }
            
            return RedirectToAction("ForumManagement");
        }

        [Authorize(Roles = "Admin,Moderator,Instructor,Member")]
        public IActionResult ForumManagement()
        {
            List<ForumModel> forums = new List<ForumModel>();
            forums = _db.Forums.ToList();

            return View(forums);
        }

        [Authorize(Roles = "Instructor,Member")]
        public IActionResult EditForum(int? Id)
        {
            var forumDetails = _db.Forums.Find(Id);

            if (forumDetails == null)
            {
                return NotFound();
            }

            return View(forumDetails);
        }

        [HttpPost]
        public IActionResult EditForumPost(ForumModel forumDetails)
        {
            if (ModelState.IsValid)
            {
                _db.Forums.Update(forumDetails);
                _db.SaveChanges();

                return RedirectToAction("ForumManagement");
            }

            return View();

        }

        [Authorize(Roles = "Admin,Moderator,Instructor,Member")]
        public IActionResult DeleteForum(int? Id) {

            var forumDetails = _db.Forums.Find(Id);
            if (forumDetails == null)
            {
                return NotFound();
            }

            var forumImages = _db.ForumImages.Where(img => img.Forum_Id == Id).ToList();
            ViewBag.ForumImages = forumImages;

            return View(forumDetails);
        }

        public IActionResult DelFunction(int? Id)
        {
            var forumDel = _db.Forums.Find(Id);
            _db.Forums.Remove(forumDel);
            _db.SaveChanges();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteForumPost(int? Id, List<string> selectedFileNames)
        {
            var forumDetails = _db.Forums.Find(Id);
            if (forumDetails == null)
            {
                return NotFound();
            }

                using (var amazonS3client = new AmazonS3Client("ASIA55H4D3RU4A4FHPPY", "iNLGeewV9yUxA6IuNkyMOemy8AsNbjlZLZ7viMnu", "FwoGZXIvYXdzECoaDP3L8HjwKXk8UPlC1yK8ATGPPPKzE5cHLUIh9LG3lQugAslLW7vOcDBfg5jvE66KR57LtJ0c/lpL/Zh0EzKfjXTtsswSiubbR59uH7jKLIpzMk7kn+htzWuZm3uKw/vxfByMovndFw+d/lK3ybV8RV3eoW0nhL71vcDrhtLp4k9joIwSin+6n3m4T5pBWajXPdHGVpg7n+DfRNDTrDTTlbZJlOV5xIoQGzITpOSa0bHJ1OXaDdkQlrKufn7EcO1pmz6LjOdbFb0/dpwFKJCmlqoGMi3chmlUe5k0/qK3R/BpyOJvURk+gNPxMcMEH6p5vXyB9LN/polJQZBVKB+2svk=", RegionEndpoint.USEast1))
                {
                    foreach (var fileName in selectedFileNames)
                    {
                        System.Diagnostics.Debug.WriteLine("filename" + fileName );

                        await amazonS3client.DeleteObjectAsync(new DeleteObjectRequest()
                        {
                            BucketName = "healthyhavens3",
                            Key = fileName
                        });

                            var forumImage = _db.ForumImages.FirstOrDefault(x => x.Forum_Id == Id);
                     
                            _db.ForumImages.Remove(forumImage);
                            _db.SaveChanges();
                    }
                    
                }

            _db.Forums.Remove(forumDetails);
            _db.SaveChanges();

            return RedirectToAction("ForumManagement");
        }

        
        public IActionResult ViewForum(int? Id)
        {
            var forumDetails = _db.Forums.Find(Id);

            if (forumDetails == null)
            {
                return NotFound();
            }

            return View(forumDetails);
        }

    }
}
