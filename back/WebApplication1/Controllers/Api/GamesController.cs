using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public GamesController(ApplicationContext context)
        {
            _context = context;
        }

        // DTO для безопасной сериализации
        private class GameDto
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
            // Жанры можно добавить, но без циклических ссылок
            public List<string> Genres { get; set; } = new();
        }

        // GET: api/Games
        [HttpGet]
        public async Task<IActionResult> GetGames(
            [FromQuery] string? search = null,
            [FromQuery] string? category = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 24)
        {
            // 🔒 Валидация параметров
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.Games.AsQueryable();

            // 🔍 Фильтрация по поиску (игнорируем null/пустые)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalizedSearch = search.Trim();
                query = query.Where(g =>
                    !string.IsNullOrEmpty(g.Title) &&
                    g.Title.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(category) &&
     !string.Equals(category, "all", StringComparison.OrdinalIgnoreCase))
            {
                var normalizedCategory = category.Trim().ToLower();
                query = query.Where(g =>
                    !string.IsNullOrEmpty(g.Category) &&
                    g.Category.ToLower() == normalizedCategory);
            }

            // ⚠️ Обязательная сортировка для стабильной пагинации
            query = query.OrderBy(g => g.Id);

          
            var total = await query.CountAsync();

         
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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
                    Genres = g.GameGenres.Select(gg => gg.Genre.Name).ToList()
                })
                .ToListAsync();

            return Ok(new
            {
                items,
                total,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)total / pageSize),
                hasMore = (page * pageSize) < total
            });
        }

        // GET: api/Games/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetGame(int id)
        {
            var game = await _context.Games
                .Include(g => g.GameGenres)
                .ThenInclude(gg => gg.Genre)
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
                    Genres = g.GameGenres.Select(gg => gg.Genre.Name).ToList()
                })
                .FirstOrDefaultAsync();

            if (game == null)
                return NotFound();

            return Ok(game);
        }

        // POST: api/Games
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddGame([FromBody] CreateGameDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 🔍 Проверка жанров
            if (dto.GenreIds == null || !dto.GenreIds.Any())
                return BadRequest("Необходимо указать хотя бы один жанр.");

            var existingGenres = await _context.Genres
                .Where(g => dto.GenreIds.Contains(g.Id))
                .ToListAsync();

            if (existingGenres.Count != dto.GenreIds.Distinct().Count())
            {
                return BadRequest("Один или несколько жанров не найдены.");
            }

            // 🧹 Очистка входных данных
            var game = new Game
            {
                Title = dto.Title?.Trim() ?? string.Empty,
                Description = dto.Description?.Trim() ?? string.Empty,
                Price = dto.Price,
                Developer = dto.Developer?.Trim() ?? string.Empty,
                AgeRating = dto.AgeRating,
                ReleaseDate = dto.ReleaseDate,
                ImageUrl = dto.ImageUrl?.Trim(),
                Category = dto.Category?.Trim() ?? "other", // ← безопасный default
                OldPrice = dto.OldPrice,
                Tag = dto.Tag?.Trim(),
                Publisher = dto.Publisher?.Trim() ?? string.Empty,
                IsPreorder = dto.IsPreorder
            };

            if (string.IsNullOrEmpty(game.Title))
                return BadRequest("Название игры не может быть пустым.");

            // 🔗 Привязка жанров
            foreach (var genre in existingGenres)
            {
                game.GameGenres.Add(new GameGenre { GenreId = genre.Id });
            }

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            // 📤 Возвращаем DTO, а не сущность
            var gameDto = new GameDto
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                Price = game.Price,
                OldPrice = game.OldPrice,
                Tag = game.Tag,
                Developer = game.Developer,
                Publisher = game.Publisher,
                AgeRating = game.AgeRating,
                ReleaseDate = game.ReleaseDate,
                ImageUrl = game.ImageUrl,
                Category = game.Category,
                IsPreorder = game.IsPreorder,
                Genres = existingGenres.Select(g => g.Name).ToList()
            };

            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, gameDto);
        }

        // DTO для создания игры
        public class CreateGameDto
        {
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public decimal? OldPrice { get; set; }
            public string? Tag { get; set; }
            public string Developer { get; set; } = string.Empty;
            public string? Publisher { get; set; }
            public int AgeRating { get; set; }
            public DateTime ReleaseDate { get; set; }
            public string ImageUrl { get; set; } = string.Empty;
            public string Category { get; set; } = "other";
            public bool IsPreorder { get; set; }
            public List<int> GenreIds { get; set; } = new();
        }
    }
}