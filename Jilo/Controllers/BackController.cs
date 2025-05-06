using Jilo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Jilo.Controllers
{
    public class BackController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RegistrationBack()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AuthorizationBack()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AddGames()
        {
            return RedirectToAction("Index", "MainPage");
        }

        public IActionResult Exit()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            Console.WriteLine($"Токен перед выходом: {token}");

            Response.Cookies.Delete("jwt");

            Console.WriteLine("Токен удалён (куки очищены)");
            return RedirectToAction("Index", "Home");
        }

    }
    
}
