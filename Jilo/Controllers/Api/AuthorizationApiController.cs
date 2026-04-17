using Jilo.Models;
using Jilo.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;

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
        public async Task<IActionResult> Index([FromBody] AuthorizationRequest authorization)
        {
            if(!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }
           
            var user = await _context.Users.FirstOrDefaultAsync(f => f.Username == authorization.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(authorization.Password, user.Passwordhash))
            {
                return Unauthorized();
            }

            var token = GenerateJWT(user);
            AddTokenToCookie(token);

            return Ok(new
            {
                UserId = user.Id,
                Username = user.Username
            });
        }

        private void AddTokenToCookie(string token)
        {
            HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(1)
            });
        }

        private string GenerateJWT(User user)
        {
            var tokenHandler = new JsonWebTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Role, user.Role ?? string.Empty)
            };

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Audience = _configuration["Jwt:Audience"],
                Issuer = _configuration["Jwt:Issuer"],
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddDays(1)
            };

            return tokenHandler.CreateToken(tokenDescriptor);
        }
    }
}