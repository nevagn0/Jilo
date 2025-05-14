using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Jilo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Jilo.Controllers
{

    public class AuthorizationController : Controller
    {
        private readonly JiloContext _context;
        private readonly IConfiguration _config;
        public AuthorizationController(JiloContext context, IConfiguration conf)
        {
            _context = context;
            _config = conf;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Authorization(User user)
        {
            var us = await _context.Users.FirstOrDefaultAsync(f => f.Username == user.Username);

            bool IsValid = BCrypt.Net.BCrypt.Verify(user.Passwordhash, us.Passwordhash);

            if (!IsValid || us == null)
            {
                ModelState.AddModelError(string.Empty, "Пользователь не найден");
                return View("Index", user);
            }

            var token = GenerateJWT(us);
            Console.WriteLine($"Generated token: {token}");

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            });
            return RedirectToAction("Index", "MainPage");


        }

        private string GenerateJWT(User user)
        {
            var SecKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var Credentional = new SigningCredentials(SecKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: Credentional
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
