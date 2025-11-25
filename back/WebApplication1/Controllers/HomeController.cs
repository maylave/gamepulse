using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        // Для 500-ошибок (уже есть у тебя)
        public IActionResult Error() => View();

        // 👇 ДОБАВЬ ЭТО ДЕЙСТВИЕ
        public IActionResult Error404()
        {
            Response.StatusCode = 404; // важно!
            return View();
        }
    }
}