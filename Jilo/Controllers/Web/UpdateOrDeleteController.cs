using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Jilo.Dto;
using Jilo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jilo.Controllers.Web
{
    [Authorize]
    [Route("UpdateOrDelete")]
    public class UpdateOrDeleteController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JiloContext _context;
        public UpdateOrDeleteController(IHttpClientFactory contextAccessor, JiloContext context)
        {
            _httpClientFactory = contextAccessor;
            _context = context;
        }
        public IActionResult UpOrDel()
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (UserId == null)
            {
                RedirectToAction("Index","Home");
            }
            var games = _context.GamesUsers.Where(v => v.IdUser == int.Parse(UserId)).Include(v => v.IdGameNavigation).ToList();
            return View(games);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update(int idUser, int idGame, [FromForm] GamesUserUpdateDto dto)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PutAsJsonAsync(
                $"https://localhost:7136/api/Update?idUser={idUser}&idGame={idGame}",
                dto);
            return RedirectToAction("UpOrDel");
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(int idUser, int idGame)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync(
                $"https://localhost:7136/api/Update?idUser={idUser}&idGame={idGame}");
            return RedirectToAction("UpOrDel");
        }

    }
}
