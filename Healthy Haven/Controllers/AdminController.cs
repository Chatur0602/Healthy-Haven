using Healthy_Haven.Data;
using Healthy_Haven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Healthy_Haven.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
       
        private readonly UserManager<ApplicationUser> userManager; 

        public AdminController( UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
           
        }

        public IActionResult UserManagement()
        {
            var users = userManager.Users.ToList();
            return View(users); 
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}