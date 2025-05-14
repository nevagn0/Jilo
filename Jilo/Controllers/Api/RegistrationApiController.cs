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
        public async Task<IActionResult> Registration([FromForm] Registration registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (await _context.Users.AnyAsync(v => v.Email == registration.Email))
            {
                return Conflict("польщзователь с таким Email уже есть");
            }
            if (await _context.Users.AnyAsync(u => u.Username == registration.Username))
            {
                return Conflict("Пользователь с таким никнеймом уэже зарегистрирован");
            }

            string Avatar = "/uploads/foto.jpg";

            if (registration.AvatarFile != null && registration.AvatarFile.Length > 0)
            {
                var UploadsFoto = Path.Combine(_environment.WebRootPath, "uploads");

                if (!Directory.Exists(UploadsFoto))
                    Directory.CreateDirectory(UploadsFoto);

                var uniqueFileName = $"{Guid.NewGuid()}_{registration.AvatarFile.FileName}";
                var filePath = Path.Combine(UploadsFoto, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await registration.AvatarFile.CopyToAsync(fileStream);
                }
                Avatar = $"/uploads/{uniqueFileName}";
            }

            var user = new User
            {
                Username = registration.Username,
                Email = registration.Email,
                Passwordhash = BCrypt.Net.BCrypt.HashPassword(registration.Password),
                Socialcredits = 0,
                Avatar = Avatar,
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
