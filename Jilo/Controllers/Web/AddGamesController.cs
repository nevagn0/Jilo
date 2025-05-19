using System.Net.Http.Headers;
using Jilo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
[Route("AddGames")]
public class AddGamesController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JiloContext _context;

    public AddGamesController(IHttpClientFactory httpClientFactory, JiloContext context)
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
    }

    [HttpGet("AddGame")]
    public IActionResult AddGame()
    {
        var games = _context.Games.ToList();
        ViewBag.Message = TempData["Message"];
        ViewBag.MessageType = TempData["MessageType"];
        return View(games);
    }

    [HttpPost("AddGame")]
    public async Task<IActionResult> AddGame(int gameId, string? rank, string? role, string timeInGame)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("AddGame");
        }

        var existingGame = await _context.GamesUsers
            .AnyAsync(gu => gu.IdUser == int.Parse(userId) && gu.IdGame == gameId);

        if (existingGame)
        {
            TempData["Message"] = "У вас уже есть эта игра в профиле";
            TempData["MessageType"] = "error";
            return RedirectToAction("AddGame");
        }

        var token = HttpContext.Request.Cookies["jwt"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("AddGame");
        }

        var client = _httpClientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("api/games/add-to-user", new
        {
            GameId = gameId,
            Rank = rank,
            Role = role,
            TimeInGame = timeInGame
        });

        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Игра успешно добавлена!";
            TempData["MessageType"] = "success";
            return RedirectToAction("AddGame");
        }

        TempData["Message"] = await response.Content.ReadAsStringAsync();
        TempData["MessageType"] = "error";
        return RedirectToAction("AddGame");
    }
}