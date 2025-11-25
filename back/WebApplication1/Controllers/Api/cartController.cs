using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public CartController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<ActionResult<PagedResult<GameDto>>> GetGames(
    string? category = null,
    int page = 1,
    int limit = 24)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 24;
            if (limit > 100) limit = 100;

            var query = _context.Games.AsQueryable();

            if (!string.IsNullOrWhiteSpace(category) &&
                !string.Equals(category, "all", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(g => g.Category == category);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(g => new GameDto
                {
                    Id = g.Id,
                    Title = g.Title,
                    Price = g.Price,
                    ImageUrl = g.ImageUrl,
                    Category = g.Category
                })
                .ToListAsync();

            var result = new PagedResult<GameDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = limit
            };

            return Ok(result);
        }
        // GET: api/cart
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == user.Id)
                .Include(c => c.Game)
                .ToListAsync();

            return cartItems.Select(c => new CartItemDto
            {
                Id = c.Id,
                GameId = c.GameId,
                Quantity = c.Quantity,
                Game = new GameDto
                {
                    Id = c.Game.Id,
                    Title = c.Game.Title,
                    Price = c.Game.Price,
                    ImageUrl = c.Game.ImageUrl
                }
            }).ToList();
        }

        // POST: api/cart
        [HttpPost]
        public async Task<ActionResult<CartItemDto>> AddToCart([FromBody] AddToCartDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var game = await _context.Games.FindAsync(dto.GameId);
            if (game == null)
                return NotFound("Игра не найдена");

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.GameId == dto.GameId);

            СartItem cartItem;
            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                cartItem = existingItem;
            }
            else
            {
                cartItem = new СartItem
                {
                    GameId = dto.GameId,
                    Quantity = dto.Quantity,
                    UserId = user.Id
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();


            return new CartItemDto
            {
                Id = cartItem.Id,
                GameId = cartItem.GameId,
                Quantity = cartItem.Quantity,
                Game = new GameDto
                {
                    Id = cartItem.Game.Id,
                    Title = cartItem.Game.Title,
                    Price = cartItem.Game.Price,
                    ImageUrl = cartItem.Game.ImageUrl
                }
            };
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CartItemDto>> UpdateCartItem(int id, [FromBody] int quantity)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var cartItem = await _context.CartItems
                .Include(c => c.Game)
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == user.Id);

            if (cartItem == null)
                return NotFound();

            if (quantity <= 0)
                return BadRequest("Количество должно быть больше 0");

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();

            return new CartItemDto
            {
                Id = cartItem.Id,
                GameId = cartItem.GameId,
                Quantity = cartItem.Quantity,
                Game = new GameDto
                {
                    Id = cartItem.Game.Id,
                    Title = cartItem.Game.Title,
                    Price = cartItem.Game.Price,
                    ImageUrl = cartItem.Game.ImageUrl
                }
            };
        }
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class AddToCartDto
        {
            public int GameId { get; set; }
            public int Quantity { get; set; } = 1;
        }

        public class CartItemDto
        {
            public int Id { get; set; }
            public int GameId { get; set; }
            public int Quantity { get; set; }
            public GameDto Game { get; set; } = null!;
        }

        public class GameDto
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public string? ImageUrl { get; set; }

            public string? Category { get; set; }
        }
        public class PagedResult<T>
        {
            public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
            public int TotalCount { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        }
    }
}