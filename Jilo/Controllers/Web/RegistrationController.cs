using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Jilo.Models;
using System.ComponentModel.DataAnnotations;
namespace Jilo.Controllers.Web
{
    [Route("Registration")]
    public class RegistrationController : Controller
    {
        private readonly JiloContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        public RegistrationController(JiloContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Index(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var client = _httpClientFactory.CreateClient();
                var content = new MultipartFormDataContent();

                content.Add(new StringContent(model.Username), "Username");
                content.Add(new StringContent(model.Email), "Email");
                content.Add(new StringContent(model.Password), "Password");

                if (model.AvatarFile != null && model.AvatarFile.Length > 0)
                {
                    var fileContent = new StreamContent(model.AvatarFile.OpenReadStream());
                    content.Add(fileContent, "AvatarFile", model.AvatarFile.FileName);
                }
                client.BaseAddress = new Uri("https://localhost:7136");
                var registration = await client.PostAsync("api/registration/new-user", content);

                if (!registration.IsSuccessStatusCode)
                {
                    var error = await registration.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", error);
                    return View(model);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка регистрации: {ex.Message}");
                return View(model);
            }
        }
    }
}
public class RegisterViewModel
{
    public string Username { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public IFormFile? AvatarFile { get; set; }
}