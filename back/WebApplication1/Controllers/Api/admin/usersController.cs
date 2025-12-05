using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using WebApplication1.Models;

namespace WebApplication1.Controllers.Api.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase 
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager; // ← ИСПРАВЛЕНО

        public UsersController(
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager) // ← ИСПРАВЛЕНО
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var allUsers = await _userManager.Users.ToListAsync();
            var users = new List<UserDto>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                users.Add(new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Roles = roles.ToList(),
                    EmailConfirmed = user.EmailConfirmed
                });
            }

            return Ok(users);
        }

        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required");
            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Password is required");

            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                return BadRequest("User with this email already exists");

            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name ?? dto.Email,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                if (!await _roleManager.RoleExistsAsync(dto.Role))
                    return BadRequest($"Role '{dto.Role}' does not exist");

                await _userManager.AddToRoleAsync(user, dto.Role);
            }

            return Ok(new { id = user.Id });
        }

        [HttpPut("{id}/roles")]
        public async Task<ActionResult> UpdateUserRoles(int id, [FromBody] UpdateRolesDto dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound("User not found");

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (dto.Roles?.Any() == true)
            {
                foreach (var role in dto.Roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                        return BadRequest($"Role '{role}' does not exist");
                }
                await _userManager.AddToRolesAsync(user, dto.Roles);
            }

            return NoContent();
        }

        [HttpPut("{id}/password")]
        public async Task<ActionResult> ResetPassword(int id, [FromBody] ResetPasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound("User not found");

            if (string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest("New password is required");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound("User not found");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return NoContent();
        }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public bool EmailConfirmed { get; set; }
    }

    public class CreateUserDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Role { get; set; }
    }

    public class UpdateRolesDto
    {
        public List<string> Roles { get; set; } = new();
    }

    public class ResetPasswordDto
    {
        public string NewPassword { get; set; } = string.Empty;
    }
}