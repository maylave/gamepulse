using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers.Api.Admin
{
    [Route("api/addGame")]
    [ApiController]
    [Authorize(Roles = "SuperUser, Admin")]
    public class addgamesController : ControllerBase 
    {
        private readonly ApplicationContext _context;

        public addgamesController(ApplicationContext context) 
        {
            _context = context;
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
            public string ReleaseDate { get; set; } 
            public int AgeRating { get; set; }
            public bool IsPreorder { get; set; }
            public string? Developer { get; set; }
            public string? Publisher { get; set; }
            public string? ExternalUrl { get; set; } // ← Новое поле

            public List<int> GenreIds { get; set; } = new();
            public List<CreateMediaDto> Media { get; set; } = new();
        }

        public class CreateMediaDto
        {
            public string Url { get; set; } = string.Empty;
            public string Type { get; set; } = "image"; 
        }

        [HttpPost]
        public async Task<ActionResult> AddGame([FromBody] CreateGameDto dto)
        {
            
            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("Title is required.");

            if (dto.Price < 0)
                return BadRequest("Price cannot be negative.");

            if (dto.OldPrice.HasValue && dto.OldPrice < dto.Price)
                return BadRequest("OldPrice must be greater than Price.");

            if (dto.AgeRating < 0 || dto.AgeRating > 18)
                return BadRequest("AgeRating must be between 0 and 18.");

          
            if (!DateTime.TryParse(dto.ReleaseDate, out var releaseDate))
                return BadRequest("Invalid date format. Use ISO format (e.g., 2025-12-31).");

          
            if (dto.GenreIds == null || !dto.GenreIds.Any())
                return BadRequest("At least one genre is required.");

            var existingGenreIds = await _context.Genres
                .Where(g => dto.GenreIds.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync();

            if (existingGenreIds.Count != dto.GenreIds.Distinct().Count())
                return BadRequest("One or more genres do not exist.");

           
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
                ExternalUrl = dto.ExternalUrl?.Trim()
            };

           
            foreach (var genreId in dto.GenreIds.Distinct())
            {
                game.GameGenres.Add(new GameGenre { GenreId = genreId });
            }

           
            if (dto.Media != null)
            {
                foreach (var mediaDto in dto.Media)
                {
                    if (string.IsNullOrWhiteSpace(mediaDto.Url)) 
                        continue;

                    var type = mediaDto.Type?.ToLowerInvariant() switch
                    {
                        "video" => "video",
                        _ => "image"
                    };

                    game.Media.Add(new GameMedia
                    {
                        Url = mediaDto.Url.Trim(),
                        Type = type
                    });
                }
            }

            
            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction("AddGame", new { id = game.Id }, new { id = game.Id, message = "Game created successfully." });
        }
    }
}