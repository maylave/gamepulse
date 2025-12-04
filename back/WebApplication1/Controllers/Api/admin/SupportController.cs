using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using System.Security.Claims;

namespace WebApplication1.Controllers.Api
{
    [ApiController]
    [Route("api/support")]
    [Authorize(Roles = "Support, Admin")]
    public class SupportController : ControllerBase
    {
        private readonly ApplicationContext _db;

        public SupportController(ApplicationContext db)
        {
            _db = db;
        }

        // Получить список активных чатов
        [HttpGet("chats")]
        public async Task<ActionResult<List<ChatSessionAdminDto>>> GetActiveChats()
        {
            var chats = await _db.ChatSessions
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.LastActivity)
                .Select(c => new ChatSessionAdminDto
                {
                    Id = c.Id,
                    ClientName = c.ClientName,
                    LastMessage = c.Messages.Any() ? c.Messages.OrderByDescending(m => m.SentAt).First().Content : "",
                    LastMessageTime = c.Messages.Any() ? c.Messages.OrderByDescending(m => m.SentAt).First().SentAt.ToString("HH:mm") : ""
                })
                .ToListAsync();

            return Ok(chats);
        }

        // Получить сообщения по ID чата
        [HttpGet("chats/{chatId:guid}/messages")]
        public async Task<ActionResult<List<ChatMessageAdminDto>>> GetMessages(Guid chatId)
        {
            var messages = await _db.ChatMessages
                .Where(m => m.ChatSessionId == chatId)
                .OrderBy(m => m.SentAt)
                .Select(m => new ChatMessageAdminDto
                {
                    Sender = m.SenderRole,
                    Text = m.Content,
                    Time = m.SentAt.ToString("HH:mm")
                })
                .ToListAsync();

            return Ok(messages);
        }

        // Отправить ответ от поддержки
        [HttpPost("chats/{chatId:guid}/reply")]
        public async Task<IActionResult> SendReply(Guid chatId, [FromBody] ReplyRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Сообщение не может быть пустым.");

            var session = await _db.ChatSessions.FindAsync(chatId);
            if (session == null || !session.IsActive)
                return NotFound("Чат не найден или закрыт.");

            // Получаем ID текущего пользователя (админа/поддержки)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int supportUserId))
                return Unauthorized("Не удалось определить пользователя.");

            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                ChatSessionId = chatId,
                SenderRole = "support",
                Content = request.Content.Trim(),
                SentAt = DateTime.UtcNow,
                SupportUserId = supportUserId 
            };

            session.LastActivity = DateTime.UtcNow;
            _db.ChatMessages.Add(message);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }

    // DTOs
    public class ChatSessionAdminDto
    {
        public Guid Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public string LastMessageTime { get; set; } = string.Empty;
    }

    public class ChatMessageAdminDto
    {
        public string Sender { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
    }

    public class ReplyRequest
    {
        public string Content { get; set; } = string.Empty;
    }
}