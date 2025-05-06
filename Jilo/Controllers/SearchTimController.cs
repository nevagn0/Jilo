using Jilo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Jilo.Controllers
{
    public class SearchTimController : Controller
    {
        private readonly JiloContext  _context;
        public SearchTimController(JiloContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SearchUser(string push) 
        {
            var users = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(push))
            {
                users = users.Where(u => u.Username.Contains(push));
            }
            return View(users.ToList());
        }
        [HttpGet]
        public IActionResult UserFind(string name)
        {
            var user = _context.Users.Include(v => v.GamesUsers).ThenInclude(b => b.IdGameNavigation).FirstOrDefault(n => n.Username == name);
            if (user == null)
            {
                return NotFound();
            }
            
            return View(user);
        }

    }
}
