using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;
using WebApplication1.Services;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace WebApplication1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            UserManager<User> userManager,
            IConfiguration configuration,
            IEmailService emailService,
            ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
            _logger = logger;
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
                Name = dto.Name,
                EmailConfirmed = false // важно: не подтверждён
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(string.Join("; ", errors));
            }

            // Генерация 6-значного кода
            var code = new Random().Next(100000, 999999).ToString();
            user.EmailConfirmationCode = code;

            try
            {
                // Отправка email
                await _emailService.SendEmailAsync(
                    dto.Email,
                    "Подтверждение email — GamePulse",
                    code
                );

                // Сохраняем код в БД
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return StatusCode(500, "Не удалось сохранить код подтверждения.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка отправки email для {Email}", dto.Email);
                await _userManager.DeleteAsync(user);
                return StatusCode(500, new { error = "Не удалось отправить email. Попробуйте позже." });
            }

            return StatusCode(201, new
            {
                user.Id,
                user.Name,
                user.Email,
                user.AvatarUrl,
                needsEmailConfirmation = true
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

            if (!user.EmailConfirmed)
                return Unauthorized("Email не подтверждён. Проверьте почту.");

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

       [HttpPost("confirm-email")]
public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto dto)
{
    _logger.LogInformation("Начало подтверждения email: {Email}, код: {Code}", dto.Email, dto.Code);

    if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Code))
    {
        _logger.LogWarning("Пустой email или код");
        return BadRequest("Email и код обязательны.");
    }

    var user = await _userManager.FindByEmailAsync(dto.Email);
    if (user == null)
    {
        _logger.LogWarning("Пользователь не найден: {Email}", dto.Email);
        return BadRequest("Пользователь не найден.");
    }

    _logger.LogInformation("Найден пользователь: Id={Id}, EmailConfirmed={Confirmed}, Code={Code}", 
        user.Id, user.EmailConfirmed, user.EmailConfirmationCode);

    if (user.EmailConfirmed)
    {
        _logger.LogWarning("Email уже подтверждён: {Email}", dto.Email);
        return BadRequest("Email уже подтверждён.");
    }

    if (user.EmailConfirmationCode != dto.Code)
    {
        _logger.LogWarning("Неверный код: ожидался {Expected}, получен {Actual}", 
            user.EmailConfirmationCode, dto.Code);
        return BadRequest("Неверный код подтверждения.");
    }

    user.EmailConfirmed = true;
    user.EmailConfirmationCode = null;

    var updateResult = await _userManager.UpdateAsync(user);
    if (!updateResult.Succeeded)
    {
        _logger.LogError("Ошибка обновления пользователя: {Errors}", 
            string.Join(", ", updateResult.Errors));
        return StatusCode(500, "Не удалось подтвердить email.");
    }

    _logger.LogInformation("Email успешно подтверждён: {Email}", dto.Email);

    if (!await _userManager.IsInRoleAsync(user, "User"))
        await _userManager.AddToRoleAsync(user, "User");

    return Ok(new { message = "Email успешно подтверждён!" });
}
        // === ПОВТОРНАЯ ОТПРАВКА КОДА ===
        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email обязателен.");

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("Пользователь не найден.");

            if (user.EmailConfirmed)
                return BadRequest("Email уже подтверждён.");

            // Генерация НОВОГО кода
            var newCode = new Random().Next(100000, 999999).ToString();
            user.EmailConfirmationCode = newCode;

            try
            {
                await _emailService.SendEmailAsync(
                    dto.Email,
                    "Подтверждение email — GamePulse",
                    newCode
                );

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return StatusCode(500, "Не удалось сохранить новый код.");

                return Ok(new { message = "Код подтверждения отправлен повторно." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка повторной отправки email для {Email}", dto.Email);
                return StatusCode(500, "Не удалось отправить email.");
            }
        }

        // DTO для повторной отправки

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

        // === НАЗНАЧЕНИЕ АДМИНА ===
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

public class ConfirmEmailDto
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

// ➕ ДОБАВЬ ЭТОТ DTO В КОНЕЦ СЕКЦИИ
public class ResendConfirmationDto
{
    public string Email { get; set; } = string.Empty;
}
    }
}