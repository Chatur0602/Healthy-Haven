using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
//using ApplicationDbContext.Data;
//using ApplicationUser.Models;


namespace Healthy_Haven.Controllers
{
    public class ConsultationsController : Controller
    {
        public IActionResult ConsultationsManagement()
        {
            List<ConsultationsEntity> consultations = new List<ConsultationsEntity>();
            consultations = _db.Consultations.ToList();
            return View(consultations);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ConsultationsEntity consultationsDetails)
        {
            if (ModelState.IsValid)
            {
                _db.Consultations.Add(consultationsDetails);
                _db.SaveChanges();
                return RedirectToAction("ConsultationsManagement");
            }
            return View();
        }

        public IActionResult Edit(int? Id)
        {
            var consultationsDetails = _db.Consultations.Find(Id);
            if (consultationsDetails == null)
            {
                return NotFound(consultationsDetails);
            }


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
                return NotFound(consultationsDetails);
            }


            return View(consultationsDetails);
        }

        [HttpPost]
        public IActionResult DeleteConsultations(int? Id)
        {
            var consultationsDetails = _db.Consultations.Find(Id);
            if (consultationsDetails == null)
            {
                return NotFound(consultationsDetails);
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
