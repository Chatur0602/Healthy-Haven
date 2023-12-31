using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using Amazon.SimpleNotificationService;
using System;
using Amazon.SimpleNotificationService.Model;

namespace Healthy_Haven.Controllers
{
    public class ConsultationsController : Controller
    {
        private readonly ILogger<ConsultationsController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAmazonSimpleNotificationService _snsClient;

        public ConsultationsController(ILogger<ConsultationsController> logger, ApplicationDbContext db, UserManager<ApplicationUser> userManager, IAmazonSimpleNotificationService snsClient)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _snsClient = snsClient;
        }


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
                // Admins|Moderators can select both members and instructors
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
        public async Task<IActionResult> CreateConsultations(ConsultationsEntity consultationsDetails)
        {
            if (ModelState.IsValid)
            {
                // Set instructor_id based on the current user's role
                if (User.IsInRole("Instructor"))
                {
                    consultationsDetails.instructor_id = _userManager.GetUserId(User);
                }

                // Set student_id based on the current user's role
                if (User.IsInRole("Member"))
                {
                    consultationsDetails.student_id = _userManager.GetUserId(User);
                }

                _db.Consultations.Add(consultationsDetails);
                _db.SaveChanges();

                var user = await _userManager.GetUserAsync(User);

                string message = $"{{ \"message\": \" {user.Email} You have successfully booked consultation. Consultation ID: {consultationsDetails.id}\" }}";

                string subject = $"Consultation Booking";
                string snsTopicArn = "arn:aws:sns:us-east-1:712338159638:Lambda";

                await _snsClient.PublishAsync(new PublishRequest
                {
                    Message = message,
                    Subject = subject,
                    TopicArn = snsTopicArn
                });

                return RedirectToAction("ConsultationsManagement");
            }

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

                // Set the selected member based on the consultationsDetails
                ViewBag.SelectedMember = consultationsDetails.student_id;
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

                // Set the selected instructor based on the consultationsDetails
                ViewBag.SelectedInstructor = consultationsDetails.instructor_id;
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

            return View(consultationsDetails);
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Moderator,Instructor,Member")]
        public IActionResult EditConsultations(ConsultationsEntity consultationsDetails)
        {
            if (ModelState.IsValid)
            {
                // Set instructor_id based on the current user's role
                if (User.IsInRole("Instructor"))
                {
                    consultationsDetails.instructor_id = _userManager.GetUserId(User);
                }

                // Set student_id based on the current user's role
                if (User.IsInRole("Member"))
                {
                    consultationsDetails.student_id = _userManager.GetUserId(User);
                }

                // Check if the selected date is in the future
                if (consultationsDetails.date < DateTime.Now)
                {
                    ModelState.AddModelError("consultationsDetails.date", "Please select a future date.");

                    // Provide members and instructors for repopulating the dropdowns
                    ViewBag.Members = _userManager.GetUsersInRoleAsync("Member").Result
                        .Select(user => new SelectListItem { Value = user.Id, Text = $"{user.FirstName} {user.LastName}" })
                        .ToList();

                    ViewBag.Instructors = _userManager.GetUsersInRoleAsync("Instructor").Result
                        .Select(user => new SelectListItem { Value = user.Id, Text = $"{user.FirstName} {user.LastName}" })
                        .ToList();

                    return View(consultationsDetails);
                }

                _db.Consultations.Update(consultationsDetails);
                _db.SaveChanges();
                return RedirectToAction("ConsultationsManagement");
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


        [Authorize(Roles = "Admin,Moderator,Instructor,Member")]
        public IActionResult DeleteConsultations(int? Id)
        {
            var consultationsDetails = _db.Consultations.Find(Id);
            if (consultationsDetails == null)
            {
                return NotFound();
            }

            
            var student = _userManager.FindByIdAsync(consultationsDetails.student_id).Result;
            var instructor = _userManager.FindByIdAsync(consultationsDetails.instructor_id).Result;

           
            ViewBag.StudentName = $"{student.FirstName} {student.LastName}";
            ViewBag.InstructorName = $"{instructor.FirstName} {instructor.LastName}";

            return View("ConfirmDeleteConsultations", consultationsDetails);
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Moderator,Instructor,Member")]
        public IActionResult ConfirmDeleteConsultations(int? Id)
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


      

        public IActionResult Index()
        {
            return View();
        }
    }
}
