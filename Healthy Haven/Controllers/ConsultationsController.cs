using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace Healthy_Haven.Controllers
{
    public class ConsultationsController : Controller
    {
        private readonly ILogger<ConsultationsController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ConsultationsController(ILogger<ConsultationsController> logger, ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }

        // ... (other actions)

        [Authorize(Roles = "Admin,Moderator,Instructor,Member")]
        public IActionResult ConsultationsManagement()
        {
            List<ConsultationsEntity> consultations = _db.Consultations.ToList();
            return View(consultations);
        }

        [Authorize(Roles = "Admin,Moderator,Instructor,Member")]
        public IActionResult CreateConsultations()
        {
            var currentUser = _userManager.GetUserAsync(User).Result;

            if (User.IsInRole("Instructor"))
            {
                // Instructors can only select members
                var members = _userManager.GetUsersInRoleAsync("Member").Result;
                ViewBag.Members = members
                    .Select(user => new SelectListItem { Value = user.Id, Text = $"{user.FirstName} {user.LastName}" })
                    .ToList();

                ViewBag.Instructors = new List<SelectListItem>
        {
            new SelectListItem { Value = currentUser.Id, Text = $"{currentUser.FirstName} {currentUser.LastName}" }
        };
            }
            else if (User.IsInRole("Member"))
            {
                // Members can only select instructors
                var instructors = _userManager.GetUsersInRoleAsync("Instructor").Result;
                ViewBag.Instructors = instructors
                    .Select(user => new SelectListItem { Value = user.Id, Text = $"{user.FirstName} {user.LastName}" })
                    .ToList();

                ViewBag.Members = new List<SelectListItem>
        {
            new SelectListItem { Value = currentUser.Id, Text = $"{currentUser.FirstName} {currentUser.LastName}" }
        };
            }
            else
            {
                // Admins/Moderators can select both members and instructors
                var members = _userManager.GetUsersInRoleAsync("Member").Result;
                ViewBag.Members = members
                    .Select(user => new SelectListItem { Value = user.Id, Text = $"{user.FirstName} {user.LastName}" })
                    .ToList();

                var instructors = _userManager.GetUsersInRoleAsync("Instructor").Result;
                ViewBag.Instructors = instructors
                    .Select(user => new SelectListItem { Value = user.Id, Text = $"{user.FirstName} {user.LastName}" })
                    .ToList();
            }

            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Moderator,Instructor,Member")]
        public IActionResult CreateConsultations(ConsultationsEntity consultationsDetails)
        {
            if (ModelState.IsValid)
            {
                // You can access the current user's ID and set it in the consultationsDetails object
                var currentUserId = _userManager.GetUserId(User);
                consultationsDetails.instructor_id = currentUserId; // Assuming you want to set the instructor_id

                _db.Consultations.Add(consultationsDetails);
                _db.SaveChanges();
                return RedirectToAction("ConsultationsManagement");
            }

            // If the model state is not valid, return the view with the model to display validation errors
            // You may also need to repopulate ViewBag.Members and ViewBag.Instructors if needed

            var members = _userManager.GetUsersInRoleAsync("Member").Result;
            var instructors = _userManager.GetUsersInRoleAsync("Instructor").Result;

            ViewBag.Members = members
                .Select(user => new SelectListItem { Value = user.Id, Text = $"{user.FirstName} {user.LastName}" })
                .ToList();

            ViewBag.Instructors = instructors
                .Select(user => new SelectListItem { Value = user.Id, Text = $"{user.FirstName} {user.LastName}" })
                .ToList();

            return View(consultationsDetails);
        }



        [Authorize(Roles = "Admin,Moderator,Instructor,Member")]
        public IActionResult EditConsultations(int? id)
        {
            var consultationsDetails = _db.Consultations.Find(id);
            if (consultationsDetails == null)
            {
                return NotFound();
            }

            var members = _userManager.GetUsersInRoleAsync("Member").Result;
            var instructors = _userManager.GetUsersInRoleAsync("Instructor").Result;

            ViewBag.Members = members
                .Select(user => new SelectListItem { Value = user.Id, Text = $"{user.FirstName} {user.LastName}" })
                .ToList();

            ViewBag.Instructors = instructors
                .Select(user => new SelectListItem { Value = user.Id, Text = $"{user.FirstName} {user.LastName}" })
                .ToList();

            return View(consultationsDetails);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Moderator,Instructor,Member")]
        public IActionResult EditConsultations(ConsultationsEntity consultationsDetails)
        {
            if (ModelState.IsValid)
            {
                // You can access the current user's ID and set it in the consultationsDetails object
                var currentUserId = _userManager.GetUserId(User);
                consultationsDetails.instructor_id = currentUserId; // Assuming you want to set the instructor_id

                _db.Consultations.Update(consultationsDetails);
                _db.SaveChanges();
                return RedirectToAction("ConsultationsManagement");
            }

            // If the model state is not valid, return the view with the model to display validation errors
            // You may also need to repopulate ViewBag.Members and ViewBag.Instructors if needed

            var members = _userManager.GetUsersInRoleAsync("Member").Result;
            var instructors = _userManager.GetUsersInRoleAsync("Instructor").Result;

            ViewBag.Members = members
                .Select(user => new SelectListItem { Value = user.Id, Text = $"{user.FirstName} {user.LastName}" })
                .ToList();

            ViewBag.Instructors = instructors
                .Select(user => new SelectListItem { Value = user.Id, Text = $"{user.FirstName} {user.LastName}" })
                .ToList();

            return View(consultationsDetails);
        }


        // ... (other actions)

        [Authorize(Roles = "Admin,Moderator,Instructor,Member")]
        public IActionResult DeleteConsultations(int? Id)
        {
            var consultationsDetails = _db.Consultations.Find(Id);
            if (consultationsDetails == null)
            {
                return NotFound();
            }

            _db.Consultations.Remove(consultationsDetails);
            _db.SaveChanges();

            return RedirectToAction("ConsultationsManagement");
        }

        // ... (other actions)

        public IActionResult Index()
        {
            return View();
        }
    }
}
