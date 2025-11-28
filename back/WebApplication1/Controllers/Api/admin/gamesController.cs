using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers.Api.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class GamesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public GamesController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<GameDto>>> GetGames()
        {
            var games = await _context.Games
                .Include(g => g.GameGenres) // нужно, чтобы получить GenreId
                .ToListAsync();

            var dtos = games.Select(g => new GameDto
            {
                Id = g.Id,
                Title = g.Title,
                Description = g.Description,
                Price = g.Price,
                OldPrice = g.OldPrice,
                Tag = g.Tag,
                ImageUrl = g.ImageUrl,
                Category = g.Category,
                ReleaseDate = g.ReleaseDate,
                AgeRating = g.AgeRating,
                IsPreorder = g.IsPreorder,
                Developer = g.Developer,
                Publisher = g.Publisher,
                GenreIds = g.GameGenres.Select(gg => gg.GenreId).ToList()
            }).ToList();

            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GameDto>> GetGame(int id)
        {
            var game = await _context.Games
                .Include(g => g.GameGenres)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
                return NotFound();

            return new GameDto
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                Price = game.Price,
                OldPrice = game.OldPrice,
                Tag = game.Tag,
                ImageUrl = game.ImageUrl,
                Category = game.Category,
                ReleaseDate = game.ReleaseDate,
                AgeRating = game.AgeRating,
                IsPreorder = game.IsPreorder,
                Developer = game.Developer,
                Publisher = game.Publisher,
                GenreIds = game.GameGenres.Select(gg => gg.GenreId).ToList()
            };
        }

        [HttpPost]
        public async Task<ActionResult> CreateGame([FromBody] CreateGameDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("Title is required.");
            if (dto.GenreIds == null || !dto.GenreIds.Any())
                return BadRequest("At least one genre is required.");

            if (!DateTime.TryParse(dto.ReleaseDate, out var releaseDate))
                return BadRequest("Invalid date format. Use YYYY-MM-DD.");

            var existingGenreIds = await _context.Genres
                .Where(g => dto.GenreIds.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync();

            if (existingGenreIds.Count != dto.GenreIds.Count)
                return BadRequest("One or more genres not found.");

            var game = new Game
            {
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                OldPrice = dto.OldPrice,
                Tag = dto.Tag,
                ImageUrl = dto.ImageUrl,
                Category = dto.Category,
                ReleaseDate = releaseDate.Kind == DateTimeKind.Utc
    ? releaseDate
    : releaseDate.ToUniversalTime(),
                AgeRating = dto.AgeRating,
                IsPreorder = dto.IsPreorder,
                Developer = dto.Developer,
                Publisher = dto.Publisher,
                GameGenres = dto.GenreIds.Select(id => new GameGenre { GenreId = id }).ToList()
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return Ok(new { id = game.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateGame(int id, [FromBody] CreateGameDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var game = await _context.Games
                .Include(g => g.GameGenres)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
                return NotFound();

            // Валидация даты
            if (string.IsNullOrWhiteSpace(dto.ReleaseDate) ||
                !DateTime.TryParse(dto.ReleaseDate, out var releaseDate))
            {
                return BadRequest("Неверный формат даты релиза. Используйте YYYY-MM-DD.");
            }

            // Валидация жанров
            var existingGenreIds = await _context.Genres
                .Where(g => dto.GenreIds.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync();

            if (existingGenreIds.Count != dto.GenreIds.Count)
                return BadRequest("Один или несколько жанров не найдены.");

            // Обновление полей
            game.Title = dto.Title;
            game.Description = dto.Description;
            game.Price = dto.Price;
            game.OldPrice = dto.OldPrice;
            game.Tag = dto.Tag;
            game.ImageUrl = dto.ImageUrl;
            game.Category = dto.Category;
            game.ReleaseDate = releaseDate;
            game.AgeRating = dto.AgeRating;
            game.IsPreorder = dto.IsPreorder;
            game.Developer = dto.Developer;
            game.Publisher = dto.Publisher;

            // Обновление жанров
            _context.GameGenres.RemoveRange(game.GameGenres);
            game.GameGenres = dto.GenreIds.Select(gid => new GameGenre { GenreId = gid }).ToList();

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
                return NotFound();

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    // Например, в папке Dtos/ или прямо в контроллере
    public class GameDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string? Tag { get; set; }
        public string? ImageUrl { get; set; }
        public string? Category { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int AgeRating { get; set; }
        public bool IsPreorder { get; set; }
        public string? Developer { get; set; }
        public string? Publisher { get; set; }

        // Только ID жанров — никаких объектов!
        public List<int> GenreIds { get; set; } = new();
    }

    // DTO для входящих данных — ReleaseDate как string!
    public class CreateGameDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }          // null разрешён
        public string? Tag { get; set; }
        public string? ImageUrl { get; set; }
        public string? Category { get; set; }
        public string? ReleaseDate { get; set; }       // ← строка!
        public int AgeRating { get; set; }
        public bool IsPreorder { get; set; }
        public string? Developer { get; set; }
        public string? Publisher { get; set; }
        public List<int> GenreIds { get; set; } = new();
    }
}