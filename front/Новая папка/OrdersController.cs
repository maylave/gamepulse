using DateAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationContext _context;
        public OrdersController(ApplicationContext context) => _context = context;

        [HttpGet]
        public IActionResult Get() => Ok(_context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Game)
            .ToList());

        [HttpGet("{id}")]
        public IActionResult Get(int id) => _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Game)
            .FirstOrDefault(o => o.Id == id) is { } o ? Ok(o) : NotFound();

        [HttpPost]
        public IActionResult Post([FromBody] CreateOrderDto dto)
        {
            if (dto.UserId <= 0 || !dto.Items.Any())
                return BadRequest("Пользователь и товары обязательны.");

            if (_context.Users.Find(dto.UserId) == null)
                return BadRequest("Пользователь не найден.");

            decimal total = 0;
            var order = new Order { UserId = dto.UserId, OrderDate = DateTime.Now, TotalAmount = 0 };
            _context.Orders.Add(order);
            _context.SaveChanges(); // чтобы получить order.Id

            foreach (var item in dto.Items)
            {
                var game = _context.Games.Find(item.GameId);
                if (game == null) continue;
                if (item.Quantity <= 0) continue;

                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    GameId = item.GameId,
                    Quantity = item.Quantity,
                    Price = game.Price
                });
                total += game.Price * item.Quantity;
            }

            order.TotalAmount = total;
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return NotFound();
            _context.Orders.Remove(order);
            _context.SaveChanges();
            return NoContent();
        }
    }

    public class CreateOrderDto
    {
        public int UserId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int GameId { get; set; }
        public int Quantity { get; set; }
    }
}