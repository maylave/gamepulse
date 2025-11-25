using Microsoft.AspNetCore.Mvc;
using DateAccess;

namespace WebApplication1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationContext _context;
        public GenresController(ApplicationContext context) => _context = context;

        [HttpGet] public IActionResult Get() => Ok(_context.Genres.ToList());
        [HttpGet("{id}")] public IActionResult Get(int id) => _context.Genres.Find(id) is { } g ? Ok(g) : NotFound();
        [HttpPost]
        public IActionResult Post([FromBody] GenreDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Название обязательно.");
            var genre = new Genre { Name = dto.Name };
            _context.Genres.Add(genre);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = genre.Id }, genre);
        }
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] GenreDto dto)
        {
            var genre = _context.Genres.Find(id);
            if (genre == null) return NotFound();
            if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Название обязательно.");
            genre.Name = dto.Name;
            _context.SaveChanges();
            return Ok(genre);
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var genre = _context.Genres.Find(id);
            if (genre == null) return NotFound();
            _context.Genres.Remove(genre);
            _context.SaveChanges();
            return NoContent();
        }
    }

    public class GenreDto { public string Name { get; set; } = ""; }
}