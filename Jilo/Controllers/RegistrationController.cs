using Jilo.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
namespace Jilo.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly JiloContext _context;
        public RegistrationController(JiloContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Index(User user, IFormFile avatarFile)
        {
            if (user.Username == null || user.Username.Length > 30) 
            {
                ModelState.AddModelError("Username", "Никнейм не может быть больше 30 или быть пустым");
                return View("Index",user);
            }
            if(user.Email == null)
            {
                ModelState.AddModelError("Email", "Email не может быть пустым");
                return View("Index", user);
            }
            if (user.Passwordhash == null || user.Passwordhash.Length < 8) 
            {
                ModelState.AddModelError("Passwordhash", "пароль не может быть ментше 8");
                return View("Index", user);
            }
            if (avatarFile != null && avatarFile.Length > 0) 
            {
                string UploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                string UniqueFile = Guid.NewGuid().ToString() + '_' + avatarFile.FileName;
                string filePath = Path.Combine(UploadFolder,UniqueFile);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(fileStream);
                }
                user.Avatar = "/uploads/" + UniqueFile;
            }
            else
            {
                user.Avatar = "/uploads/foto";
            }
            user.Passwordhash = BCrypt.Net.BCrypt.HashPassword(user.Passwordhash, 12);
            user.DataRegistration = DateOnly.FromDateTime(DateTime.Now);
            user.Socialcredits = 0.0;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index","Home");
        }
    }
}
