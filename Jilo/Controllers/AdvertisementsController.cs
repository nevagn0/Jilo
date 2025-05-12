using System.Security.Claims;
using Jilo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jilo.Controllers
{
    [Authorize]
    public class AdvertisementsController : Controller
    {
        private readonly JiloContext _jiloContext;
        public AdvertisementsController(JiloContext jiloContext)
        {
            _jiloContext = jiloContext;
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.AllGames = _jiloContext.Games.ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Advertisement adver, List<int> SelectIdGame)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (user == null)
            {
                return Unauthorized();
            }
            if (!int.TryParse(user, out int userIdInt))
            {
                return BadRequest("Invalid user ID");
            }
            adver.IdUser = userIdInt;
            adver.DateCreate = DateTime.Now;

            adver.AdverstmentCreates = SelectIdGame?.Select(gameId => new AdverstmentCreate { IdGame = gameId }).ToList();

            _jiloContext.Add(adver);
            await _jiloContext.SaveChangesAsync();

            ViewBag.AllGames = await _jiloContext.Games.ToListAsync();
            return View(adver);
        }

    }
}
