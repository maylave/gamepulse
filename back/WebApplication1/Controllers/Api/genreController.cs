using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class genreController : ControllerBase
    {

        private readonly ApplicationContext _context;

        public genreController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]

       
        public async Task<IActionResult> GetAll()
        {
            var genres = await _context.Genres.ToListAsync();
            return Ok(genres); // возвращает 200 + JSON-массив жанров
        }

        
    }
}
