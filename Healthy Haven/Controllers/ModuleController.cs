using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Healthy_Haven.Views.Course;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace Healthy_Haven.Controllers
{
    public class ModuleController : Controller
    {
        private readonly ILogger<ModuleController> _logger;
        private readonly ApplicationDbContext _db;

        public ModuleController(ILogger<ModuleController> logger, ApplicationDbContext db)
        {
            _db = db;
            _logger = logger;
        }
        public IActionResult ViewModules()
        {
            List<ModulesModel> Modules = new List<ModulesModel>();
            Modules = _db.Modules.ToList();
            return View("ViewModules", Modules);
        }

        public IActionResult UserModules()
        {
            List<ModulesModel> modules = _db.Modules.ToList();
            return View("UserModules", modules); 
        }


        public IActionResult CreateModule()
        {
            var moduledetails = new ModulesModel();
            return View("CreateModule", moduledetails);
        }

        [HttpPost]
        public IActionResult Create(ModulesModel moduledetails)
        {
            try
            {
                Debug.WriteLine("CHAPTER= " + moduledetails.chapter + "MODULE= " + moduledetails.module);
                _db.Modules.Add(moduledetails);
                _db.SaveChanges();

                return RedirectToAction("ViewModules");
            }
            catch (Exception ex)
            {
                // Handle exceptions (log or provide user feedback)
                Debug.WriteLine($"An error occurred: {ex.Message}");
                return RedirectToAction("ErrorPage"); // Redirect to an error page or handle appropriately
            }
        }

        public IActionResult Edit(int? id)
        {
            var moduledetails = _db.Modules.Find(id);
            if (moduledetails == null)
            {
                return NotFound();
            }
            return View("EditModule", moduledetails);
        }

        [HttpPost]
        public IActionResult Edit(ModulesModel moduledetails)
        {
            _db.Modules.Update(moduledetails);
            _db.SaveChanges();
            return RedirectToAction("ViewModules");
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var moduledetails = _db.Modules.Find(id);
            if (moduledetails == null)
            {
                return NotFound();
            }

            return View("DeleteModule", moduledetails);
        }

        [HttpPost]
        public IActionResult DeleteModule(int? id)
        {
            var moduledetails = _db.Modules.Find(id);

            if (id == null)
            {
                return NotFound();
            }

            if (moduledetails == null)
            {
                return NotFound();
            }

            _db.Modules.Remove(moduledetails);
            _db.SaveChanges();

            return RedirectToAction("ViewModules");
        }
    }
}
