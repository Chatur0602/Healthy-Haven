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


            using (var amazonS3client = new AmazonS3Client("ASIA55H4D3RU3YGBSRC2", "b9NK4j9Q1Yr06QA6pGtwSM3o27h4JqOXoby+mbV+", "FwoGZXIvYXdzEGgaDBkMuLt8g08U5gPcmCK8AVVCxej8nXNSwFsaB07hFdFhgb2B+b+bXB2hKP7i5VSlUrnOS/IrdwSMmLXuLsW/LZKUc1r/dViFnptCHvL0orWYtKi7w/GPF6Ik6fWu5SsJTErRuFiuAqBdYry/0vdcvbYidn0xz0Xatl1aaLn0BeUzvaxORNIRUNDmTtwNAhvUaqjn29VmCJ4MiYKIL9W3ZqilUdXjMq9K32xaTDiF9rF/SGRtvPBDxAybhvcCSAkVDRKrpfI4if//OQMPKJ2a3KoGMi34qE6qpuveWUmolNzGHL6RCp7cGa61r/99fFE12NIbnVTlqLWLIjaXESbM5yU=", RegionEndpoint.USEast1))
            {
                foreach (var fileName in selectedFileNames)
                {
                    System.Diagnostics.Debug.WriteLine("filename" + fileName);

                    // Construct the full key with folder path
                    var folderPath = "ForumImages/";
                    var key = folderPath + fileName;

                    await amazonS3client.DeleteObjectAsync(new DeleteObjectRequest()
                    {
                        BucketName = "healthyhavens3",
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

            string message = $"Your Forum was deleted either due to the copyright or censorship issues";

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