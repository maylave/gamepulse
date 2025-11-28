// WebApplication1/Controllers/Api/UsersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public UsersController(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // === РЕГИСТРАЦИЯ ===
        [HttpPost]
        
        public async Task<IActionResult> Post([FromBody] CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password) ||
                string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Email, пароль и имя обязательны.");
            }

            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                return BadRequest("Email уже используется.");

            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name
                // AvatarUrl — по умолчанию
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(string.Join("; ", errors));
            }

            await _userManager.AddToRoleAsync(user, "User");

            return StatusCode(201, new
            {
                user.Id,
                user.Name,
                user.Email,
                user.AvatarUrl
            });
        }

        // === ВХОД ===
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email и пароль обязательны.");

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Неверный email или пароль.");

            var roles = await _userManager.GetRolesAsync(user);

          
            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection["Key"] ?? throw new InvalidOperationException("JWT Key is missing.");
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                id = user.Id,
                name = user.Name,
                email = user.Email,
                avatarUrl = user.AvatarUrl,
                roles,
                token = tokenString
            });
        }

        // === ПРОФИЛЬ: GET ===
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            if (!TryGetUserId(out int userId))
                return BadRequest("Требуется X-User-Id в заголовке (демо)");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                user.Id,
                user.Name,
                user.Email,
                user.AvatarUrl,
                roles
            });
        }

        // === ПРОФИЛЬ: PATCH ===
        [HttpPatch("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            if (!TryGetUserId(out int userId))
                return BadRequest("Требуется X-User-Id в заголовке (демо)");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.Name))
                user.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.AvatarUrl))
                user.AvatarUrl = dto.AvatarUrl;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest("Не удалось обновить профиль.");

            return Ok(new
            {
                user.Id,
                user.Name,
                user.Email,
                user.AvatarUrl
            });
        }

        // === НАЗНАЧЕНИЕ АДМИНА (ТОЛЬКО ДЛЯ ДЕМО/ТЕСТА!) ===
        [HttpPost("make-admin")]
        public async Task<IActionResult> MakeAdmin([FromBody] MakeAdminDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return NotFound("Пользователь не найден");

            var result = await _userManager.AddToRoleAsync(user, "Admin");
            if (!result.Succeeded)
                return BadRequest("Не удалось назначить роль Admin");

            return Ok("Пользователь стал админом");
        }

        // === ПРОВЕРКА РОЛИ ===
        [HttpGet("check-role")]
        public async Task<IActionResult> CheckRole()
        {
            if (!TryGetUserId(out int userId))
                return BadRequest("Нет X-User-Id");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new
            {
                id = user.Id,
                name = user.Name,
                roles
            });
        }

        // === ВСПОМОГАТЕЛЬНЫЙ МЕТОД ===
        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            if (!Request.Headers.TryGetValue("X-User-Id", out var header) ||
                !int.TryParse(header, out userId))
                return false;
            return true;
        }

        // === DTO ===
        public class LoginDto
        {
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
        }

        public class CreateUserDto
        {
            public string Name { get; set; } = "";
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
        }

        public class UpdateProfileDto
        {
            public string Name { get; set; } = "";
            public string AvatarUrl { get; set; } = "";
        }

        public class MakeAdminDto
        {
            public string Email { get; set; } = string.Empty;
        }
    }
}