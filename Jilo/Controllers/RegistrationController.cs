using Microsoft.AspNetCore.Mvc;

namespace Jilo.Controllers
{
    public class RegistrationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Registration()
        {

            return RedirectToAction("Index","Home");
        }
    }
}
