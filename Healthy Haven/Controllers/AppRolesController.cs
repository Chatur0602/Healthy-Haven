using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Healthy_Haven.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AppRolesController : Controller
    {
        public readonly RoleManager<IdentityRole> _roleManager;

        public AppRolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        //List All Roles
        public IActionResult RoleManagement()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(IdentityRole model)
        {
            if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
            }

            return RedirectToAction("RoleManagement");
        }

        [HttpGet]
        public IActionResult DeleteRole(String? Id)
        {
            var role = _roleManager.FindByNameAsync(Id);

            return View();

        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(IdentityRole model)
        {
            if (_roleManager.RoleExistsAsync(model.Id).GetAwaiter().GetResult())
            {
                _roleManager.DeleteAsync(model);
            }

            return RedirectToAction("RoleManagement");
        }
    }
}
