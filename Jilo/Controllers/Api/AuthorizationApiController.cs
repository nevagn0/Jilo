using Jilo.Models;
using Jilo.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Jilo.Controllers.Api
{
    [ApiController]
    [Route("api/authorization")]
    public class AuthorizationApiController : ControllerBase
    {
        private readonly JiloContext _context;
        private readonly IConfiguration _configuration;
        public AuthorizationApiController(JiloContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpPost("autorization-user")]
        public async Task<IActionResult> Index([FromBody] Authorization authorization)
        {
            if(ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }
            var us = await _context.Users.FirstOrDefaultAsync(f => f.Username == authorization.Username);

            if (us == null || !BCrypt.Net.BCrypt.Verify(authorization.Password, us.Passwordhash))
            {
                return BadRequest(ModelState);
            }
            var token = GenerateJWT(us);
            Console.WriteLine($"Generated token: {token}");

            
            return Ok(new
            {
                Token = token,
                UserId = us.Id,
                Username = us.Username
            });
        }
        private string GenerateJWT(User user)
        {
            var SecKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var Credentional = new SigningCredentials(SecKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: Credentional
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}