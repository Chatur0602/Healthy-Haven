using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Healthy_Haven.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<UserEntity> users= new List<UserEntity>();
            users = _db.Users.ToList();

            return View(users);
        }

        public IActionResult AddUser(UserEntity userDetails)
        {
            if (ModelState.IsValid)
            {
                _db.Users.Add(userDetails); 
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}