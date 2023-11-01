using Amazon.S3.Transfer;
using Amazon.S3;
using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Amazon;

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
                // Add forum details to the database after successfully processing files
                forumDetails.User_Id = user.Id; // Set the user ID
                forumDetails.Created_At = DateTime.Now; // Set the date of creation
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

                    foreach (var file in files)
                    {
                        if (file != null && file.Length > 0)
                        {
                            if (file.Length > 5 * 1024 * 1024) // 5MB
                            {
                                ViewBag.Error = "File size exceeds the limit (5MB).";
                                break;
                            }
                            else if (totalSize > totalSizeLimit)
                            {
                                ViewBag.Error = "Total file size exceeds the limit (5MB).";
                                break;
                            }
                            else if (fileCount > maxFileCount)
                            {
                                ViewBag.Error = "Exceeded the maximum allowed files (5).";
                                break;
                            }
                            else if (!allowedImageTypes.Contains(file.ContentType))
                            {
                                ViewBag.Error = "Invalid file type. Only image files (JPEG, PNG, GIF) are allowed.";
                                break;
                            }
                            else {

                                // Continue processing the file as in your original code
                                using (var amazonS3client = new AmazonS3Client("ASIA55H4D3RUZGROEEHP", "BKLeX4bwKE888oRoZJ8lH7811IMXCra2roTOIgn0", "FwoGZXIvYXdzEPb//////////wEaDB2hA8TOQTddBV0lkiK8AXAYUmz8GN5rH6l0SiMFkObIWTadTMGmAvulGFN/oRnVV5rwNqj5cDgFiixiv3uSex1Hhuh2VU3l9633ARGb4AuQNVNO+MxUo7XHAy6fA6yqzIdB8z3m8kDNyf9xEdBc3WR8troAH+4YyNbUUXQQhJ34oNJb1ZxNB2blMWMDPazxet4bg0OMvfxKT4RD/B5HnT7Y+lsSdG7Cl477agKQPXr8ci2b9OuXjU1q7n+ayaEsQhBr/Dm3Fyl5oBdrKPzmiqoGMi2o6u1sDb0DJWjwwd/3ZcxAJStFFoPELu0WSpiwEGaM2qOKEbtl+feXkhuGnoU=", RegionEndpoint.USEast1))
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
                                        }
                                    }
                                }
                            }
                        fileCount++;
                        totalSize += totalSize;
                        }
                    // Check for model state errors
                    if (!string.IsNullOrEmpty(ViewBag.Error))
                    {
                        // Handle validation errors and don't add forum details
                        return View(forumDetails);
                    }
                }
            }
            _db.SaveChanges();
            return RedirectToAction("ForumDashboard");
        }

   



     

        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult ForumManagement()
        {
            List<ForumModel> forums = new List<ForumModel>();
            forums = _db.Forums.ToList();

            return View(forums);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult DeleteForum(int? Id) {

            var forumDetails = _db.Forums.Find(Id);
            if (forumDetails == null)
            {
                return NotFound();
            }
            return View(forumDetails);        
        }

        [HttpPost]
        public IActionResult DeleteForumPost(int? Id)
        {
            var forumDetails = _db.Forums.Find(Id);
            if (forumDetails == null)
            {
                return NotFound();
            }

            _db.Forums.Remove(forumDetails);
            _db.SaveChanges();

            return RedirectToAction("ForumManagement");
        }

    }
}
