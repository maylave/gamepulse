    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using WebApplication1.Models;
  
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public WishlistController(ApplicationContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var wishlistGameIds = await _context.WishlistItems // ← ИСПРАВЛЕНО: WishlistItems
                .Where(w => w.UserId == user.Id)
                .Select(w => w.GameId)
                .ToListAsync();

            if (!wishlistGameIds.Any())
                return Ok(new List<GameDto>());

            var games = await _context.Games
                .Where(g => wishlistGameIds.Contains(g.Id))
                .Include(g => g.GameGenres).ThenInclude(gg => gg.Genre)
                .Select(g => new GameDto
                {
                    Id = g.Id,
                    Title = g.Title ?? string.Empty,
                    Description = g.Description ?? string.Empty,
                    Price = g.Price,
                    OldPrice = g.OldPrice,
                    Tag = g.Tag,
                    Developer = g.Developer ?? string.Empty,
                    Publisher = g.Publisher,
                    AgeRating = g.AgeRating,
                    ReleaseDate = g.ReleaseDate,
                    ImageUrl = g.ImageUrl,
                    Category = g.Category,
                    IsPreorder = g.IsPreorder,
                    ExternalUrl = g.ExternalUrl,
                    Genres = g.GameGenres.Select(gg => gg.Genre.Name).ToList()
                })
                .ToListAsync();

            return Ok(games);
        }

        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleWishlist([FromBody] ToggleWishlistDto dto)
        {
            if (dto.GameId <= 0)
                return BadRequest("Некорректный ID игры");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // ← ИСПРАВЛЕНО: user.Id — это string, но у тебя UserId в WishlistItem — int!
            // Это критическая проблема! Смотрим ниже 👇

            var existing = await _context.WishlistItems // ← ИСПРАВЛЕНО
                .FirstOrDefaultAsync(w => w.UserId == user.Id && w.GameId == dto.GameId);

            if (existing != null)
            {
                _context.WishlistItems.Remove(existing); // ← ИСПРАВЛЕНО
                await _context.SaveChangesAsync();
                return Ok(new { isInWishlist = false });
            }
            else
            {
                var gameExists = await _context.Games.AnyAsync(g => g.Id == dto.GameId);
                if (!gameExists)
                    return NotFound("Игра не найдена");

                _context.WishlistItems.Add(new WishlistItem // ← ИСПРАВЛЕНО
                {
                    UserId = user.Id, // ← ОПАСНО! Типы не совпадают!
                    GameId = dto.GameId,
                    AddedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
                return Ok(new { isInWishlist = true });
            }
        }

        public class ToggleWishlistDto
        {
            public int GameId { get; set; }
        }

        // Внутренний DTO (или вынеси в отдельный файл)
        public class GameDto
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public decimal? OldPrice { get; set; }
            public string? Tag { get; set; }
            public string Developer { get; set; } = string.Empty;
            public string? Publisher { get; set; }
            public int AgeRating { get; set; }
            public DateTime ReleaseDate { get; set; }
            public string? ImageUrl { get; set; }
            public string? Category { get; set; }
            public bool IsPreorder { get; set; }
            public string? ExternalUrl { get; set; }
            public List<string> Genres { get; set; } = new();
        }
    }
}