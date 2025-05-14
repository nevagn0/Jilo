
using Jilo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jilo.Controllers
{
    [Authorize]
    public class AddAdversmetController : Controller
    {
        private readonly JiloContext _context;

        public AddAdversmetController(JiloContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            var games = _context.Games.ToList();
            ViewBag.game = games;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateVersment(string discription, int selectedGameId)
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return NotFound("Пользователь не найден.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(v => v.Username == username);
            if (user == null)
            {
                return NotFound("Пользователь не найден в базе данных.");
            }

            var game = await _context.Games.FindAsync(selectedGameId);
            if (game == null)
            {
                return NotFound("Игра не найдена.");
            }

            var adverstment = new AdversmetUser
            {
                IdUser = user.Id,
                IdGame = selectedGameId,
                Discription = discription,
                DateCreate = DateTime.Now,
                IdSecondUser = null
            };

            try
            {
                await _context.AddAsync(adverstment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при создании объявления: {ex.Message}");
            }

            var games = await _context.Games.ToListAsync();
            ViewBag.game = games;
            return View("Create", games);
        }
    }
}
