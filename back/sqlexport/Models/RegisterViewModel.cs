// WebApplication1/Models/RegisterRequest.cs
namespace WebApplication1.Models
{
    public class RegisterViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}