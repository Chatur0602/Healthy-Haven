using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Healthy_Haven.Controllers
{
    [Authorize(Roles = "Admin,Moderator")]
    public class AdminController : Controller
    {
       
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _db;

        public AdminController( UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            this.userManager = userManager;
            _db = db;
           
        }

        public IActionResult UserManagement()
        {
            var users = userManager.Users.ToList();
         
            return View(users); 
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(String? Id) {

            var userDetails = _db.Users.Find(Id);
            if(userDetails == null) {
                return NotFound();
            }

            return View (userDetails); 
        
        }

        [HttpPost]
        public IActionResult DeleteUserPost(String? Id)
        {
            var userDetails = _db.Users.Find(Id);
            if(userDetails == null)
            {
                return NotFound();  
            }

            _db.Users.Remove(userDetails);  
            _db.SaveChanges();  

            return RedirectToAction("UserManagement");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}