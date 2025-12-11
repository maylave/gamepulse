using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")] // → /api/dev
    public class DevController : ControllerBase // ← рекомендую PascalCase: DevController
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public DevController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

 
        [HttpDelete("purchases/clear")]
        [Authorize]
        public async Task<IActionResult> ClearPurchases()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var purchases = await  _context.Purchases
                .Where(p => p.UserId == user.Id)
                .ToListAsync();

            _context.Purchases.RemoveRange(purchases);
            await _context.SaveChangesAsync();

            return Ok("Очистка завершена");
        }
    }
}