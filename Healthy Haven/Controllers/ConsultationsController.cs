using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectListItem
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;



namespace Healthy_Haven.Controllers
{
    public class ConsultationsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ConsultationsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult ConsultationsManagement()
        {
            List<ConsultationsEntity> consultations = _db.Consultations.ToList();
            return View(consultations);
        }

        public IActionResult Create()
        {
            var members = _userManager.GetUsersInRoleAsync("Member").Result;

            // Get users with the "Instructor" role
            var instructors = _userManager.GetUsersInRoleAsync("Instructor").Result;

            // Populate the ViewBag.Members and ViewBag.Instructors with lists of SelectListItem
            ViewBag.Members = members
                .Select(user => new SelectListItem { Value = user.Id, Text = $"{user.FirstName} {user.LastName}" })
                .ToList();

            ViewBag.Instructors = instructors
                .Select(user => new SelectListItem { Value = user.Id, Text = $"{user.FirstName} {user.LastName}" })
                .ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(ConsultationsEntity consultationsDetails)
        {
            
            {
                _db.Consultations.Add(consultationsDetails);
                _db.SaveChanges();
                return RedirectToAction("ConsultationsManagement");
            }
            return View();
        }

        public IActionResult Edit(int? id)
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
        public IActionResult Edit(ConsultationsEntity consultationsDetails)
        {
            if (ModelState.IsValid)
            {
                _db.Consultations.Update(consultationsDetails);
                _db.SaveChanges();
                return RedirectToAction("ConsultationsManagement");
            }

            return View();
        }

        public IActionResult Delete(int? Id)
        {
            var consultationsDetails = _db.Consultations.Find(Id);
            if (consultationsDetails == null)
            {
                return NotFound();
            }

            return View(consultationsDetails);
        }

        [HttpPost]
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



        public IActionResult Index()
        {
            return View();
        }
    }
}
