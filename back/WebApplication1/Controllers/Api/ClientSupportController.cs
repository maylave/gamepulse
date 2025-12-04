// Controllers/Api/ClientSupportController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using System.Security.Claims;

namespace WebApplication1.Controllers.Api
{
    [ApiController]
    [Route("api/client/support")]
    [Authorize] // Только авторизованные пользователи
    public class ClientSupportController : ControllerBase
    {
        private readonly ApplicationContext _db;

        public ClientSupportController(ApplicationContext db)
        {
            _db = db;
        }

        // Получить или создать чат для текущего пользователя
        [HttpGet("chat")]
        public async Task<ActionResult<ChatSessionDto>> GetOrCreateChat()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return NotFound();

            // Ищем активный чат
            var chat = await _db.ChatSessions
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.ClientId == userId && c.IsActive);

            if (chat == null)
            {
                // Создаём новый чат
                chat = new ChatSession
                {
                    ClientId = userId,
                    ClientName = user.Name ?? $"User{userId}",
                    CreatedAt = DateTime.UtcNow,
                    LastActivity = DateTime.UtcNow,
                    IsActive = true
                };
                _db.ChatSessions.Add(chat);
                await _db.SaveChangesAsync();

                // Приветственное сообщение от поддержки (опционально)
                _db.ChatMessages.Add(new ChatMessage
                {
                    ChatSessionId = chat.Id,
                    SenderRole = "support",
                    Content = "Здравствуйте! Чем можем помочь?",
                    SentAt = DateTime.UtcNow
                });
                await _db.SaveChangesAsync();
            }

            return new ChatSessionDto
            {
                Id = chat.Id,
                Messages = chat.Messages.OrderBy(m => m.SentAt).Select(m => new ChatMessageDto
                {
                    Sender = m.SenderRole,
                    Text = m.Content,
                    Time = m.SentAt.ToString("HH:mm")
                }).ToList()
            };
        }

       [HttpPost("chat/message")]
public async Task<IActionResult> SendMessage([FromBody] ClientMessageRequest request)
{
    try
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest("Сообщение не может быть пустым.");

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            return Unauthorized("Неверный токен.");

        var chat = await _db.ChatSessions
            .FirstOrDefaultAsync(c => c.ClientId == userId && c.IsActive);

        if (chat == null)
            return NotFound("Активный чат не найден.");

        _db.ChatMessages.Add(new ChatMessage
        {
            ChatSessionId = chat.Id,
            SenderRole = "user",
            Content = request.Content.Trim(),
            SentAt = DateTime.UtcNow
        });

        chat.LastActivity = DateTime.UtcNow;
        await _db.SaveChangesAsync();

       
        return Ok(new { success = true, message = "Сообщение отправлено" });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при отправке сообщения: {ex.Message}");
        return StatusCode(500, new { error = "Внутренняя ошибка сервера", details = ex.Message });
    }
}
    }

    // DTOs
    public class ChatSessionDto
    {
        public Guid Id { get; set; }
        public List<ChatMessageDto> Messages { get; set; } = new();
    }

    public class ChatMessageDto
    {
        public string Sender { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
    }

    public class ClientMessageRequest
    {
        public string Content { get; set; } = string.Empty;
    }
}