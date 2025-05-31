using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Jilo.Models;
using Jilo.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Jilo.Controllers.Web
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
            return View("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Authorization(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View("Index",loginDto);
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Passwordhash))
            {
                ModelState.AddModelError(nameof(LoginDto.Password), "Неверный пароль");
                return View("Index", loginDto);
            }

            if (user == null)
            {
                ModelState.AddModelError(nameof(LoginDto.Username), "Пользователь с таким именем не найден");
                return View("Index", loginDto);
            }

            var token = GenerateJwtToken(user);

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax, 
                Expires = DateTime.UtcNow.AddHours(1),
                IsEssential = true
            });
            Console.WriteLine($"JWT токен: {token}");
            return RedirectToAction("Index", "MainPage");
        }
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}