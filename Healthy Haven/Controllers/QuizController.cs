using Microsoft.AspNetCore.Mvc;

namespace Healthy_Haven.Controllers
{
    public class QuizController : Controller
    {
        public IActionResult QuizBuilder()
        {
            return View();
        }
    }
}
