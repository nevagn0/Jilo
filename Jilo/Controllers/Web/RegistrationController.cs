using Microsoft.AspNetCore.Mvc;
using Jilo.Models;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using Jilo.Dto;
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
        [HttpGet("verify-username")]
        public IActionResult VerifyUsername(string username)
        {
            if (_context.Users.Any(u => u.Username == username))
            {
                return Json($"Имя пользователя {username} уже занято");
            }
            return Json(true);
        }
        [HttpGet("verify-email")]
        public IActionResult VerifyEmail(string email)
        {
            if (_context.Users.Any(u => u.Email == email))
            {
                return Json($"Email {email} уже зарегистрирован");
            }
            return Json(true);
        }
        [HttpPost]
        public async Task<IActionResult> Index(RegistrationRequest model, string returnUrl = null)
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
                    Console.WriteLine($"ФАААААААААААААААААААААААЙЛ: {model.AvatarFile.FileName}");
                    var fileContent = new StreamContent(model.AvatarFile.OpenReadStream());
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(model.AvatarFile.ContentType);
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

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl); 
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
    [Remote(action: "VerifyUsername", controller: "Registration", AdditionalFields = "__RequestVerificationToken", ErrorMessage = "Это имя уже занято")]
    public string Username { get; set; }

    [Remote(action: "VerifyEmail", controller: "Registration", AdditionalFields = "__RequestVerificationToken", ErrorMessage = "Этот email уже зарегистрирован")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Пароль обязателен")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Пароль должен быть не менее 8 символов")]
    public string Password { get; set; }

    public IFormFile? AvatarFile { get; set; }
}