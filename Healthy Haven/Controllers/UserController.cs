using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Healthy_Haven.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _db;

        public UserController(ILogger<UserController> logger, ApplicationDbContext db)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult UserManagement()
        {
            List<UserEntity> users = new List<UserEntity>();
            users = _db.Users.ToList();

            return View(users);
        }


        public IActionResult AddUser(UserEntity userDetails)
        {
            if (ModelState.IsValid)
            {
                _db.Users.Add(userDetails);
                _db.SaveChanges();
                
                return RedirectToAction("UserManagement");
            }
            
            return View();
        }

        public IActionResult EditUser(int? Id)
        {
            var userDetails = _db.Users.Find(Id);
            
            if(userDetails == null)
            {
                return NotFound();
            }
           
            return View(userDetails);

        }

        [HttpPost]
        public IActionResult EditUserPost(UserEntity userDetails)
        {
            if (ModelState.IsValid)
            {
                _db.Users.Update(userDetails);
                _db.SaveChanges();
                
                return RedirectToAction("UserManagement");
            }
           
            return View();

        }

        public IActionResult DeleteUser(int? Id)
        {
            var userDetails = _db.Users.Find(Id);
            
            if (userDetails == null)
            {
                return NotFound();
            }
            
            return View(userDetails);

        }

        [HttpPost]
        public IActionResult DeleteUserPost(int? Id)
        {
            var userDetails = _db.Users.Find(Id);
            
            if (userDetails == null)
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