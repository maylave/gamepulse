using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers.Api.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class GenreController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public GenreController(ApplicationContext context)
        {
            _context = context;
        }

    
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> GetAll()
        {
            var genres = await _context.Genres
                .AsNoTracking()
                .ToListAsync();
            return Ok(genres);
        }

      
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Genre>> GetById(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
                return NotFound();

            return Ok(genre);
        }

        
        [HttpPost]
        public async Task<ActionResult<Genre>> Create([FromBody] CreateGenreDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var genre = new Genre { Name = dto.Name };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = genre.Id }, genre);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGenreDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
                return NotFound();

            genre.Name = dto.Name;
            _context.Entry(genre).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
                return NotFound();

            // Проверка: используется ли жанр в играх?
            var usedInGames = await _context.GameGenres.AnyAsync(gg => gg.GenreId == id);
            if (usedInGames)
                return BadRequest("Нельзя удалить жанр, который используется в играх.");

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GenreExists(int id)
        {
            return _context.Genres.Any(e => e.Id == id);
        }

      
        public class CreateGenreDto
        {
            public string Name { get; set; } = string.Empty;
        }

       
        public class UpdateGenreDto
        {
            public string Name { get; set; } = string.Empty;
        }
    }
}