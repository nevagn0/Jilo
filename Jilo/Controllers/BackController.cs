using Jilo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Jilo.Controllers
{
    public class BackController : Controller
    {
        private readonly JiloContext _context;
        public BackController(JiloContext context)
        {
            _context = context;
        }
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
            var token = Request.Cookies["jwt"];
            Console.WriteLine($"Токен перед выходом: {token}");

            var username = User.Identity?.Name;
            Console.WriteLine($"Имя пользователя: {username}");

            var user = _context.Users.FirstOrDefault(v => v.Username == username);

            user.LastOnline = DateTime.Now;// Сохранит текущие дату и время
            Console.WriteLine($"{user.Username}");
            Console.WriteLine(user.LastOnline);
            _context.SaveChanges();

            Response.Cookies.Delete("jwt");

            Console.WriteLine("Токен удалён (куки очищены)");
            return RedirectToAction("Index", "Home");
        }

    }
    
}
