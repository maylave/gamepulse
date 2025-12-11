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
                .Include(g => g.GameGenres)
                .Include(g => g.Media) // ← Включаем медиа
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
                ExternalUrl = g.ExternalUrl, // ←
                GenreIds = g.GameGenres.Select(gg => gg.GenreId).ToList(),
                Media = g.Media.Select(m => new MediaDto // ←
                {
                    Id = m.Id,
                    Url = m.Url,
                    Type = m.Type
                }).ToList()
            }).ToList();

            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GameDto>> GetGame(int id)
        {
            var game = await _context.Games
                .Include(g => g.GameGenres)
                .Include(g => g.Media) // ←
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
                ExternalUrl = game.ExternalUrl, // ←
                GenreIds = game.GameGenres.Select(gg => gg.GenreId).ToList(),
                Media = game.Media.Select(m => new MediaDto // ←
                {
                    Id = m.Id,
                    Url = m.Url,
                    Type = m.Type
                }).ToList()
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

            if (existingGenreIds.Count != dto.GenreIds.Distinct().Count())
                return BadRequest("One or more genres not found.");

            var game = new Game
            {
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim(),
                Price = dto.Price,
                OldPrice = dto.OldPrice,
                Tag = dto.Tag?.Trim(),
                ImageUrl = dto.ImageUrl,
                Category = dto.Category?.Trim() ?? "other",
                ReleaseDate = releaseDate.Kind == DateTimeKind.Utc
                    ? releaseDate
                    : releaseDate.ToUniversalTime(),
                AgeRating = dto.AgeRating,
                IsPreorder = dto.IsPreorder,
                Developer = dto.Developer?.Trim(),
                Publisher = dto.Publisher?.Trim(),
                ExternalUrl = dto.ExternalUrl?.Trim() // ←
            };

     
            foreach (var genreId in dto.GenreIds.Distinct())
            {
                game.GameGenres.Add(new GameGenre { GenreId = genreId });
            }

  
            if (dto.Media != null)
            {
                foreach (var mediaDto in dto.Media)
                {
                    if (string.IsNullOrWhiteSpace(mediaDto.Url)) continue;

                    game.Media.Add(new GameMedia
                    {
                        Url = mediaDto.Url.Trim(),
                        Type = mediaDto.Type?.ToLowerInvariant() == "video" ? "video" : "image"
                    });
                }
            }

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, new { id = game.Id });
        }
        [HttpGet("genres")]
public async Task<IActionResult> GetGenres()
{
    var genres = await _context.Genres
        .Select(g => new { g.Id, g.Name })
        .ToListAsync();
    return Ok(genres);
}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateGame(int id, [FromBody] CreateGameDto dto)
        {
            var game = await _context.Games
                .Include(g => g.GameGenres)
                .Include(g => g.Media) // ← Обязательно!
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
                return NotFound();

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

            if (existingGenreIds.Count != dto.GenreIds.Distinct().Count())
                return BadRequest("One or more genres not found.");

            // Обновляем основные поля
            game.Title = dto.Title.Trim();
            game.Description = dto.Description?.Trim();
            game.Price = dto.Price;
            game.OldPrice = dto.OldPrice;
            game.Tag = dto.Tag?.Trim();
            game.ImageUrl = dto.ImageUrl;
            game.Category = dto.Category?.Trim() ?? "other";
            game.ReleaseDate = releaseDate.Kind == DateTimeKind.Utc
                ? releaseDate
                : releaseDate.ToUniversalTime();
            game.AgeRating = dto.AgeRating;
            game.IsPreorder = dto.IsPreorder;
            game.Developer = dto.Developer?.Trim();
            game.Publisher = dto.Publisher?.Trim();
            game.ExternalUrl = dto.ExternalUrl?.Trim(); // ←

            // Удаляем старые жанры
            _context.GameGenres.RemoveRange(game.GameGenres);
            game.GameGenres.Clear();

            // Добавляем новые жанры
            foreach (var genreId in dto.GenreIds.Distinct())
            {
                game.GameGenres.Add(new GameGenre { GenreId = genreId });
            }

            // Удаляем старые медиа
            _context.GameMedias.RemoveRange(game.Media);
            game.Media.Clear();

            // Добавляем новые медиа
            if (dto.Media != null)
            {
                foreach (var mediaDto in dto.Media)
                {
                    if (string.IsNullOrWhiteSpace(mediaDto.Url)) continue;

                    game.Media.Add(new GameMedia
                    {
                        Url = mediaDto.Url.Trim(),
                        Type = mediaDto.Type?.ToLowerInvariant() == "video" ? "video" : "image"
                    });
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _context.Games
                .Include(g => g.Media) // ← EF автоматически удалит связанные записи благодаря OnDelete(Cascade)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
                return NotFound();

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    // DTO для ответа
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
        public string? ExternalUrl { get; set; } // ←
        public List<int> GenreIds { get; set; } = new();
        public List<MediaDto> Media { get; set; } = new(); // ←
    }

    public class MediaDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Type { get; set; } = "image";
    }

   
    public class CreateGameDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string? Tag { get; set; }
        public string? ImageUrl { get; set; }
        public string? Category { get; set; }
        public string? ReleaseDate { get; set; } 
        public int AgeRating { get; set; }
        public bool IsPreorder { get; set; }
        public string? Developer { get; set; }
        public string? Publisher { get; set; }
        public string? ExternalUrl { get; set; }
        public List<int> GenreIds { get; set; } = new();
        public List<CreateMediaDto> Media { get; set; } = new(); 
    }

    public class CreateMediaDto
    {
        public string Url { get; set; } = string.Empty;
        public string Type { get; set; } = "image";
    }
}