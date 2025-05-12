using Jilo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jilo.Controllers
{
    [Authorize]
    public class AddGamesController : Controller
    {
        private readonly JiloContext _context;

        public AddGamesController(JiloContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var games = _context.Games.ToList();
            foreach (var game in games)
            {
                Console.WriteLine($"Game ID: {game.Id}, Name: {game.Name}");
            }
            return View(games);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGame(int gameId, string? rank, string? role)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    Console.WriteLine("Пользователь не аутентифицирован");
                    return RedirectToAction("Index", "Authorization");
                }

                Console.WriteLine($"Получен gameId: {gameId}");

                var game = await _context.Games.FindAsync(gameId);
                if (game == null)
                {
                    Console.WriteLine($"Игра с ID {gameId} не найдена в БД");
                    TempData["Error"] = "Игра не найдена";
                    return RedirectToAction("Index");
                }

                var username = User.Identity.Name;
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    Console.WriteLine($"Пользователь {username} не найден");
                    return RedirectToAction("Index", "Authorization");
                }


                if (await _context.GamesUsers.AnyAsync(gu => gu.IdUser == user.Id && gu.IdGame == gameId))
                {
                    TempData["Message"] = "Игра уже добавлена";
                    return RedirectToAction("Index");
                }


                var gameUser = new GamesUser
                {
                    IdUser = user.Id,
                    IdGame = gameId,
                    Rank = rank ?? "Не указано",
                    Role = role ?? "Игрок"
                };

                await _context.GamesUsers.AddAsync(gameUser);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Игра успешно добавлена";
                return RedirectToAction("Index", "MainPage");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                TempData["Error"] = "Произошла ошибка";
                return RedirectToAction("Index");
            }
        }

    }
}
