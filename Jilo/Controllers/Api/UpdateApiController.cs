using Jilo.Models;
using Jilo.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Jilo.Controllers.Api
{
    [ApiController]
    [Route("api/Update")]
    public class UpdateApiController : ControllerBase
    {
        private readonly JiloContext _context;
        
        public UpdateApiController(JiloContext context)
        {
            _context = context;
        }
        
        [HttpGet("Goida")]
        public IActionResult UpOrDel()
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (UserId == null)
            {
                return Unauthorized();
            }
            var games = _context.GamesUsers.Where(v => v.IdUser == int.Parse(UserId)).Include(v => v.IdGameNavigation).ToList();
            return Ok(games);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateGame([FromQuery] int idUser, [FromQuery] int idGame, [FromBody] GamesUserUpdateDto dto)
        {
            var game = await _context.GamesUsers
                .FirstOrDefaultAsync(g => g.IdUser == idUser && g.IdGame == idGame);

            if (game == null) return NotFound();

            game.Rank = dto.Rank;
            game.TimeInGame = dto.TimeInGame;
            game.Role = dto.Role;

            await _context.SaveChangesAsync();
            return Ok(new { Success = true });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] int idUser, [FromQuery] int idGame)
        {
            var game = await _context.GamesUsers
                .FirstOrDefaultAsync(g => g.IdUser == idUser && g.IdGame == idGame);

            if (game == null) return NotFound();

            _context.GamesUsers.Remove(game);
            await _context.SaveChangesAsync();
            return Ok(new { Success = true });
        }
    }
}
