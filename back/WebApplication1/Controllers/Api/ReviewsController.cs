using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // ← не забудь этот using
using WebApplication1.Models;

namespace WebApplication1.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly ApplicationContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<ReviewsController> _logger; // ← объявляем поле

    public ReviewsController(
        ApplicationContext context,
        UserManager<User> userManager,
        ILogger<ReviewsController> logger) // ← добавляем в конструктор
    {
        _context = context;
        _userManager = userManager;
        _logger = logger; // ← присваиваем
    }

    [HttpGet("game/{gameId:int}")]
    public async Task<IActionResult> GetByGame(int gameId)
    {
        var gameExists = await _context.Games.AnyAsync(g => g.Id == gameId);
        if (!gameExists)
        {
            _logger.LogWarning("Игра с ID {GameId} не найдена при запросе отзывов", gameId);
            return BadRequest("Игра не найдена.");
        }

        var reviews = await _context.Reviews
            .Where(r => r.GameId == gameId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new
            {
                r.Id,
                r.GameId,
                authorName = r.User.Name ?? "Аноним",
                r.Rating,
                r.Text,
                r.CreatedAt
            })
            .ToListAsync();

        return Ok(reviews);
    }

   
    [HttpPost]
public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null)
    {
        _logger.LogWarning("Попытка создать отзыв без аутентификации");
        return BadRequest("Пользователь не найден.");
    }

    // Явная проверка наличия пользователя в БД
    var existingUser = await _context.Users.FindAsync(user.Id);
    if (existingUser == null)
    {
        _logger.LogWarning("Пользователь ID {UserId} отсутствует в БД", user.Id);
        return BadRequest("Пользователь не существует.");
    }

    if (dto.Rating is < 1 or > 5)
        return BadRequest("Оценка от 1 до 5");

    if (string.IsNullOrWhiteSpace(dto.Text) || dto.Text.Length > 500)
        return BadRequest("Отзыв от 1 до 500 символов");

    if (!await _context.Games.AnyAsync(g => g.Id == dto.GameId))
        return BadRequest("Игра не найдена");

    var alreadyReviewed = await _context.Reviews
        .AnyAsync(r => r.GameId == dto.GameId && r.UserId == user.Id);

    if (alreadyReviewed)
        return BadRequest("Вы уже оставляли отзыв на эту игру");


    var review = new Review
    {
        GameId = dto.GameId,
        UserId = user.Id,
        Rating = dto.Rating,
        Text = dto.Text.Trim(),
        CreatedAt = DateTime.UtcNow
    };

    try
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Отзыв ID {ReviewId} создан пользователем ID {UserId}", 
                review.Id, user.Id);

            return CreatedAtAction(nameof(GetByGame), new { gameId = dto.GameId }, new
            {
                review.Id,
                review.GameId,
                authorName = existingUser.Name ?? "Аноним",
                review.Rating,
                review.Text,
                review.CreatedAt
            });
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Ошибка при создании отзыва для игры ID {GameId}", dto.GameId);
        return StatusCode(500, "Не удалось сохранить отзыв. Попробуйте позже.");
    }
}


    public class CreateReviewDto
    {
        public int GameId { get; set; }
        public int Rating { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}