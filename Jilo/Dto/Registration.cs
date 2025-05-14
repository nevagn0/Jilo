namespace Jilo.Dto
{
    public class Registration
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public IFormFile? AvatarFile { get; set; }
    }
}
