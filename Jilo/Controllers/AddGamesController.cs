using System.Net.Http.Headers;
using Jilo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
[Authorize]
[Route("AddGames")]
public class AddGamesController : Controller
{

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JiloContext _context;

    public AddGamesController(IHttpClientFactory httpClientFactory,JiloContext context)
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
    }
    [HttpGet("AddGame")]
    public IActionResult AddGame()
    {
        var games = _context.Games.ToList();
        return View(games);
    }

    [HttpPost("AddGame")]
    public async Task<IActionResult> AddGame(int gameId, string? rank, string? role, string timeInGame)
    {
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
            return RedirectToAction("Index", "MainPage");
        }

        TempData["Error"] = await response.Content.ReadAsStringAsync();
        return RedirectToAction("AddGame");
    }
}