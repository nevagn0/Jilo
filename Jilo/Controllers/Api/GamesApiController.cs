using Jilo.Dto;
using Jilo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
/// <summary>
/// Добавляет игру пользователю
/// </summary>
/// 

namespace Jilo.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/games")]
public class AddGamesApiController : ControllerBase
{
    private readonly JiloContext _context;

    public AddGamesApiController(JiloContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpPost("add-to-user")]
    public async Task<IActionResult> AddGameToUser([FromBody] AddGameToUserRequest request, [FromServices] ILogger<AddGamesApiController> logger)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0) return Unauthorized();

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            var gameExists = await _context.Games.AnyAsync(g => g.Id == request.GameId);

            if (!userExists || !gameExists)
                return NotFound("Пользователь или игра не найдены");

            var gameUser = new GamesUser
            {
                IdUser = userId,
                IdGame = request.GameId,
                Rank = request.Rank ?? "Новичок",
                Role = request.Role ?? "Игрок",
                TimeInGame = request.TimeInGame
            };

            _context.GamesUsers.Add(gameUser);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при добавлении игры");
            return StatusCode(500, "Ошибка сервера");
        }
    }
}
