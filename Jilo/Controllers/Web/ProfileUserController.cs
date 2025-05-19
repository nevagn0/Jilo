using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Jilo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Jilo.Controllers.Web
{
    [Authorize]
    public class ProfileUserController : Controller
    {
        private JiloContext _context;

        public ProfileUserController(JiloContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Profile(int id)
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return RedirectToAction("Index", "Authorization");
            }

            var userr = _context.Users.FirstOrDefault(u => u.Username == username);
            if (userr == null)
            {
                return RedirectToAction("Index", "Authorization");
            }
            // кол-во
            var GameCount = await _context.GamesUsers.CountAsync(i => i.IdUser == userr.Id);
            ViewBag.GameCount = GameCount;

            // сами игры
            var AllGames = await _context.GamesUsers.Where(t => t.IdUser == userr.Id).Include(t => t.IdGameNavigation).ToListAsync();
            ViewBag.AllGames = AllGames;

            ViewBag.Comments = _context.Comms
            .Where(c => c.Targetuser == userr.Id)
            .Include(c => c.IdUserNavigation)
            .ToList();

            var SocialCredits = await _context.Comms.Where(t => t.Targetuser == userr.Id).AverageAsync(t => t.Grade);
            ViewBag.SocialCredits = SocialCredits;

            ViewBag.Discription = userr.Discription;
            return View(userr);
        }

        [HttpPost]
        public async Task<IActionResult> UserDiscription(string discrip)
        {
            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound();
            }

            user.Discription = discrip;
            await _context.SaveChangesAsync();

            return RedirectToAction("Profile","ProfileUser");
        }
    }
}
