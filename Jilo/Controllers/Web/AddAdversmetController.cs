using Jilo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jilo.Controllers.Web
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
            ViewBag.Message = TempData["Message"];
            ViewBag.MessageType = TempData["MessageType"];
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

            var existingAd = await _context.AdversmetUsers
                .FirstOrDefaultAsync(a => a.IdUser == user.Id && a.IdGame == selectedGameId);

            if (existingAd != null)
            {
                TempData["Message"] = "У вас уже есть активное объявление для этой игры.";
                TempData["MessageType"] = "error";
                return RedirectToAction("Create");
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

                TempData["Message"] = "Объявление успешно создано!";
                TempData["MessageType"] = "success";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Ошибка при создании объявления: {ex.Message}";
                TempData["MessageType"] = "error";
            }

            return RedirectToAction("Create");
        }
    }
}