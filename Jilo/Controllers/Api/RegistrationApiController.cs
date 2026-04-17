using Jilo.Models;
using Jilo.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jilo.Controllers.Api
{
    [ApiController]
    [Route("api/registration")]
    public class RegistrationApiController : ControllerBase
    {
        private readonly JiloContext _context;
        private readonly IWebHostEnvironment _environment;
        
        public RegistrationApiController(JiloContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        
        [HttpPost("new-user")]
        public async Task<IActionResult> Registration([FromForm] RegistrationRequest registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.Users.AnyAsync(v => v.Email == registration.Email))
            {
                return Conflict("Электронная почта уже используется");
            }

            if (await _context.Users.AnyAsync(u => u.Username == registration.Username))
            {
                return Conflict("Никнейм занят");
            }

            string avatar = "/uploads/foto.jpg";

            if (registration.AvatarFile != null && registration.AvatarFile.Length > 0)
            {
                var uploadsFoto = Path.Combine(_environment.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsFoto))
                    Directory.CreateDirectory(uploadsFoto);

                var uniqueFileName = $"{Guid.NewGuid()}_{registration.AvatarFile.FileName}";
                var filePath = Path.Combine(uploadsFoto, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await registration.AvatarFile.CopyToAsync(fileStream);
                }
                avatar = $"/uploads/{uniqueFileName}";
            }
            Console.WriteLine($"AvatarFile received: {registration.AvatarFile != null}");
            var user = new User
            {
                Username = registration.Username,
                Email = registration.Email,
                Passwordhash = BCrypt.Net.BCrypt.HashPassword(registration.Password),
                Socialcredits = 0,
                Avatar = avatar,
                DataRegistration = DateOnly.FromDateTime(DateTime.Now),
                LastOnline = DateTime.Now,
                Role = "User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
            });
        }
    }
}
