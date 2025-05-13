using System.Security.Claims;
using System.Threading.Tasks.Dataflow;
using Jilo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jilo.Controllers
{
    public class ViewAdversmentController : Controller
    {
        private readonly JiloContext _context;
        public ViewAdversmentController(JiloContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> ViewAdverstment()
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var advertisements = await _context.AdversmetUsers
                .Include(a => a.IdGameNavigation)
                .Include(a => a.IdUserNavigation)
                .Where(a => a.IdUser != currentUserId)
                .ToListAsync();

            return View(advertisements);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RespondToAd(int idUser, int idGame)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var currentUserName = User.Identity?.Name;

            var advertisement = await _context.AdversmetUsers
                .FirstOrDefaultAsync(a => a.IdUser == idUser && a.IdGame == idGame);

            if (advertisement == null)
            {
                return NotFound();
            }

            if (advertisement.IdSecondUser != null)
            {
                TempData["Message"] = "На это объявление уже откликнулись";
                return RedirectToAction("Index");
            }

            advertisement.IdSecondUser = currentUserId;
            advertisement.NameSecondUser = currentUserName;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Вы успешно откликнулись на объявление";
            return RedirectToAction("Index","MainPage");
        }
        public async Task<IActionResult> AllUserAdversmet()
        {
            var username = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var advers = await _context.AdversmetUsers
                .Include(v => v.IdUserNavigation)
                .Include(v => v.IdGameNavigation)
                .Where(v => v.IdUser == username)
                .ToListAsync();

            ViewBag.Advers = advers;

            return View(advers);
        }
    }
}
