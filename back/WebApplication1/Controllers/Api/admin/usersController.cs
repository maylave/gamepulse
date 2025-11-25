using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models; // ← не забудь!

namespace WebApplication1.Controllers.Api.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase // ← переименуй на PascalCase
    {
        private readonly UserManager<User> _userManager; // ← User, не IdentityUser!

        public UsersController(UserManager<User> userManager) // ← User
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var allUsers = await _userManager.Users.ToListAsync(); // ← await + ToListAsync()

            var users = new List<UserDto>();
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                users.Add(new UserDto
                {
                    Id = user.Id,
                    Name = user.UserName,   // или user.Name, если ты хочешь отображаемое имя
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return Ok(users);
        }
    }

    public class UserDto
    {
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}