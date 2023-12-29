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
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SimpleNotificationService.Util;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Configuration;

namespace Healthy_Haven.Controllers
{
    public class ForumController : Controller
    {
        private readonly ILogger<ForumController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IAmazonSimpleNotificationService _snsClient;
        UserManager<ApplicationUser> _userManager;

        public ForumController(ILogger<ForumController> logger, ApplicationDbContext db, UserManager<ApplicationUser> userManager, IAmazonSimpleNotificationService snsClient)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _snsClient = snsClient;
        }

        public IActionResult ForumDashboard(string searchTerm, string sortBy)
        {
            // Get all forums from the database
            var forums = _db.Forums.ToList();

            // Filter forums based on the search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                forums = forums.Where(f => f.Title.Contains(searchTerm) || f.Description.Contains(searchTerm)).ToList();
            }

            // Sort forums based on the selected option
            switch (sortBy)
            {
                case "newToOld":
                    forums = forums.OrderByDescending(f => f.Created_At).ToList();
                    break;
                case "oldToNew":
                    forums = forums.OrderBy(f => f.Created_At).ToList();
                    break;
                case "likesLeastToMost":
                    forums = forums.OrderBy(f => _db.ForumLikes.Count(x => x.ForumId == f.Id)).ToList();
                    break;
                case "likesMostToLeast":
                    forums = forums.OrderByDescending(f => _db.ForumLikes.Count(x => x.ForumId == f.Id)).ToList();
                    break;
                // Add more cases if needed

                default:
                    forums = forums.OrderByDescending(f => f.Created_At).ToList();
                    break;
            }

            // Pass the sorted and filtered forums to the view
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
                    string[] allowedImageTypes = { "image/jpeg", "image/jpg", "image/png", "image/gif" };

                    int totalSize = 1;
                    int fileCount = 1;

                    IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                    var accessKeyId = configuration["AWSCredentials:AccessKeyId"];
                    var secretAccessKey = configuration["AWSCredentials:SecretAccessKey"];
                    var sessionToken = configuration["AWSCredentials:SessionToken"];

                    using (var s3Client = new AmazonS3Client(accessKeyId, secretAccessKey, sessionToken, Amazon.RegionEndpoint.USEast1))
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
                                    ViewBag.Error = "Invalid file type. Only image files (JPEG, JPG, PNG, GIF) are allowed.";
                                    DelFunction(forumId);
                                    break;
                                }
                                else
                                {
                                    using (var memorystream = new MemoryStream())
                                    {
                                        file.CopyTo(memorystream);

                                        var folderPath = "ForumImages/";
                                        var key = folderPath + file.FileName;

                                        var request = new TransferUtilityUploadRequest
                                        {
                                            InputStream = memorystream,
                                            Key = key,
                                            BucketName = "healthyheaven",
                                            ContentType = file.ContentType,
                                        };

                                        var transferUtility = new TransferUtility(s3Client);
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

                    string message = $"A new Forum has been created, check it out and Stay Updated";
                    string subject = "Forum Creation";
                    string snsTopicArn = "arn:aws:sns:us-east-1:712338159638:SNSExampleSample";

                    await _snsClient.PublishAsync(new PublishRequest
                    {
                        Message = message,
                        Subject = subject,
                        TopicArn = snsTopicArn
                    });



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
        public IActionResult DeleteForum(int? Id)
        {

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
            var forumImage = _db.ForumImages.Where(x => x.Forum_Id == Id).ToList();

            _db.ForumImages.RemoveRange(forumImage);
            _db.Forums.Remove(forumDel);
            _db.SaveChanges();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteForumPost(int? Id, List<string> selectedFileNames)
        {
            var forumDetails = _db.Forums.Find(Id);
            var forumComments = _db.Comments.Where(x => x.ForumId == Id).ToList();
            var forumLikes = _db.ForumLikes.Where(x => x.ForumId == Id).ToList();


            if (forumDetails == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            bool isModerator = await _userManager.IsInRoleAsync(user, "Moderator");

            IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            var accessKeyId = configuration["AWSCredentials:AccessKeyId"];
            var secretAccessKey = configuration["AWSCredentials:SecretAccessKey"];
            var sessionToken = configuration["AWSCredentials:SessionToken"];

            using (var s3Client = new AmazonS3Client(accessKeyId, secretAccessKey, sessionToken, Amazon.RegionEndpoint.USEast1))
            {
                foreach (var fileName in selectedFileNames)
                {
                    System.Diagnostics.Debug.WriteLine("filename" + fileName);

                    // Construct the full key with folder path
                    var folderPath = "ForumImages/";
                    var key = folderPath + fileName;

                    await s3Client.DeleteObjectAsync(new DeleteObjectRequest()
                    {
                        BucketName = "healthyheaven",
                        Key = key
                    });

                    var forumImage = _db.ForumImages.FirstOrDefault(x => x.Forum_Id == Id);

                    _db.ForumImages.Remove(forumImage);
                    _db.SaveChanges();
                }
            }

            _db.Comments.RemoveRange(forumComments);
            _db.ForumLikes.RemoveRange(forumLikes);
            foreach (var comment in forumComments)
            {
                var commentLikes = _db.CommentLikes.Where(x => x.CommentId == comment.Id).ToList();
                _db.CommentLikes.RemoveRange(commentLikes);
            }


            _db.Forums.Remove(forumDetails);
            _db.SaveChanges();

            string message = $"Dear {user.Email},\n\nWe regret to inform you that your forum with ID {Id} has been deleted due to copyright or censorship issues. If you have any questions or need further clarification, please don't hesitate to contact us.\n\nSincerely,\nThe Forum Management Team";

            string subject = "Forum Deletion Notification";

            string snsTopicArn = "arn:aws:sns:us-east-1:712338159638:SNSExampleSample";

            if (isModerator)
            {
                await _snsClient.PublishAsync(new PublishRequest
                {
                    Message = message,
                    Subject = subject,
                    TopicArn = snsTopicArn
                });
            }

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