using Jilo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Jilo.Controllers.Web
{
    public class SearchTimController : Controller
    {
        private readonly JiloContext _context;
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
            var username = User.Identity?.Name;
            var users = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(push))
            {
                users = users.Where(u => u.Username.Contains(push) && u.Username != username);
            }
            else
            {
                users = users.Where(u => u.Username != username);
            }
            return View(users.ToList());
        }
        [HttpGet]
        public IActionResult UserFind(string name)
        {
            var username = User.Identity?.Name;
            var user = _context.Users
            .Include(v => v.GamesUsers)
            .ThenInclude(b => b.IdGameNavigation)
            .Include(u => u.Comms)
            .FirstOrDefault(n => n.Username == name);
            if (user == null)
            {
                return NotFound();
            }
            if (username == name)
            {
                return RedirectToAction("Profile", "ProfileUser");
            }
            ViewBag.Comments = _context.Comms
            .Where(c => c.Targetuser == user.Id)
            .Include(c => c.IdUserNavigation)
            .ToList();

            var SocialCredits = _context.Comms.Where(t => t.Targetuser == user.Id).Average(t => t.Grade);
            ViewBag.SocialCredits = SocialCredits;

            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> AddComm(string comm, double grade, int targetUser)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var targetUserr = await _context.Users.FindAsync(targetUser);
                return RedirectToAction("Index", "Home", new
                {
                    returnUrl = $"/UserFind?name={Uri.EscapeDataString(targetUserr.Username)}"
                });
            }
            var username = User.Identity?.Name;
            if (username == null)
            {
                return RedirectToAction("Index", "Authorization");
            }

            var author = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (author == null)
            {
                return RedirectToAction("Index", "Authorization");
            }

            var newComm = new Comm
            {
                Comm1 = comm,
                Grade = grade,
                IdUser = author.Id,
                Targetuser = targetUser
            };

            _context.Comms.Add(newComm);
            await _context.SaveChangesAsync();


            return RedirectToAction("SearchUser");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProfile(int userId)
        {
            var username = await _context.Users.FindAsync(userId);
            if (username == null)
            {
                return NotFound();
            }

            _context.Remove(username);
            await _context.SaveChangesAsync();

            return RedirectToAction("SearchUser");
        }
    }
}
