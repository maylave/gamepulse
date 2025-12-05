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

        [HttpPost]
        public async Task<ActionResult> AddGame([FromBody] CreateGameDto dto)
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
    }
}