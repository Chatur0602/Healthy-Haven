using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Healthy_Haven.Controllers
{
    [Authorize(Roles = "Admin,Moderator")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _db;
        private readonly IAmazonSimpleNotificationService _snsClient;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext db, IAmazonSimpleNotificationService snsClient)
        {
            this.userManager = userManager;
            _db = db;
            _snsClient = snsClient;
        }

        public IActionResult UserManagement()
        {
            var users = userManager.Users.ToList();
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(string Id)
        {
            var userDetails = _db.Users.Find(Id);
            if (userDetails == null)
            {
                return NotFound();
            }

            return View(userDetails);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUserPost(string Id)
        {
            var userDetails = _db.Users.Find(Id);
            if (userDetails == null)
            {
                return NotFound();
            }
            
            var userEmail = userDetails.Email;
            _db.Users.Remove(userDetails);
            _db.SaveChanges();

            string message = "Regrettably, due to multiple account violations, we have had to deactivate your account. If you have any questions or concerns, please don't hesitate to contact our support team.";
            string subject = "Account Deactivation Notification";
            
            string snsTopicArn = "arn:aws:sns:us-east-1:your-account-id:YourSNSTopic";

            await _snsClient.PublishAsync(new PublishRequest
            {
                Message = message,
                Subject = subject,
                TopicArn = snsTopicArn
            });

            return RedirectToAction("UserManagement");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
