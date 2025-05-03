using Jilo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jilo.Controllers
{
    [Authorize]
    public class MainPageController : Controller
    {
        private readonly JiloContext _context;
        public MainPageController(JiloContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return RedirectToAction("Index", "Authorization");
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return RedirectToAction("Index", "Authorization");
            }

            return View(user);
        }
        public IActionResult Exit()
        {
            
            return RedirectToAction("Index","HomePage");
        }
    }
}
