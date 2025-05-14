using Jilo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;

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

        public async Task<IActionResult> Index()
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return RedirectToAction("Index", "Authorization");
            }
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            user.LastOnline = DateTime.Now;
            _context.SaveChanges();

            if (user == null)
            {
                return RedirectToAction("Index", "Authorization");
            }

            return View(user);
        }
        public IActionResult Exit()
        {

            return RedirectToAction("Index", "HomePage");
        }
    }
}