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
            if (dto == null || dto.GameId <= 0)
                return BadRequest("Требуется корректный GameId");

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
public async Task<ActionResult<CartItemDto>> UpdateCartItem(int id, [FromBody] UpdateCartItemDto dto)
        {
            if (dto == null || dto.Quantity <= 0)
                return BadRequest("Количество должно быть больше 0");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var cartItem = await _context.CartItems
                .Include(c => c.Game)
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == user.Id);
 if (cartItem == null)
    {
        // Логируем, что не нашли
        Console.WriteLine($"Элемент корзины {id} для пользователя {user.Id} НЕ найден");
        return NotFound("Элемент корзины не найден");
    }   

            cartItem.Quantity = dto.Quantity;
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

        // DELETE: api/cart/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == user.Id);

            if (cartItem == null)
                return NotFound("Элемент корзины не найден");

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/cart
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

        // --- DTOs ---
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
public class UpdateCartItemDto
{
    public int Quantity { get; set; }
}
        public class GameDto
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public string? ImageUrl { get; set; }
        }
    }
}