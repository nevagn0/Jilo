using System.Security.Claims;
using Jilo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Jilo.Controllers
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
        public async Task<IActionResult> Profile()
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
            return View(userr);

            
        }
    }
}
