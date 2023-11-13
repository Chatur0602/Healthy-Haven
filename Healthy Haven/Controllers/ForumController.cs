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

          
                 using (var amazonS3client = new AmazonS3Client("ASIA55H4D3RU3325MILR", "kAozOzONMwfcfQEyJzbY5eLvSgRTfsO0TH6wGN1+", "FwoGZXIvYXdzEAsaDJkjiy9TuY5ad8V7YCK8AW2YwrdvwbiDxeXO05F1lhl0hDocNaMoFYyQAvNHv2jrIeIlcLNntiQ4q/irQMN+CGkZbtMtk8INM5QSrPAvY8spT7NludUAcd9r7Q/nPU8YFRm2DnxOQZppvBDQOVvPZhjB1D0EcE35eW4WKMe7Fl8uHWxE+0hL9TJj7q6NUvlks7koRbWA2WtSC972y+/fGgbX8FlUEoFEWQXXTqN2jNftn9bysckYQmVqYIa54OA6GNFg72zq63IuMNRvKLjox6oGMi1wO2IlqLt2AG03R5ZTj2+K9WDMSwWuLv/va+gw5d8kilPi7Ypw95Al6wU0/dw=", RegionEndpoint.USEast1))
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
            var forumComments = _db.Comments.Where(x => x.ForumId == Id).ToList();
            var forumLikes = _db.ForumLikes.Where(x => x.ForumId == Id).ToList();
            

            if (forumDetails == null)
            {
                return NotFound();
            }

            using (var amazonS3client = new AmazonS3Client("ASIA55H4D3RU3325MILR", "kAozOzONMwfcfQEyJzbY5eLvSgRTfsO0TH6wGN1+", "FwoGZXIvYXdzEAsaDJkjiy9TuY5ad8V7YCK8AW2YwrdvwbiDxeXO05F1lhl0hDocNaMoFYyQAvNHv2jrIeIlcLNntiQ4q/irQMN+CGkZbtMtk8INM5QSrPAvY8spT7NludUAcd9r7Q/nPU8YFRm2DnxOQZppvBDQOVvPZhjB1D0EcE35eW4WKMe7Fl8uHWxE+0hL9TJj7q6NUvlks7koRbWA2WtSC972y+/fGgbX8FlUEoFEWQXXTqN2jNftn9bysckYQmVqYIa54OA6GNFg72zq63IuMNRvKLjox6oGMi1wO2IlqLt2AG03R5ZTj2+K9WDMSwWuLv/va+gw5d8kilPi7Ypw95Al6wU0/dw=", RegionEndpoint.USEast1))
            {
                foreach (var fileName in selectedFileNames)
                {
                    System.Diagnostics.Debug.WriteLine("filename" + fileName);

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

            _db.Comments.RemoveRange(forumComments);
            _db.ForumLikes.RemoveRange(forumLikes);
            foreach(var comment in forumComments)
            {
                var commentLikes = _db.CommentLikes.Where(x => x.CommentId == comment.Id).ToList();
                _db.CommentLikes.RemoveRange(commentLikes);
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
