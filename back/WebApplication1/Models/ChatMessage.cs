using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class ChatMessage
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ChatSessionId { get; set; }

        [ForeignKey("ChatSessionId")]
        public ChatSession ChatSession { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string SenderRole { get; set; } = "user"; // "user", "support"

        [Required]
        [MaxLength(4000)]
        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // Опционально: если хотите знать, кто отправил сообщение (админ)
        public int? SupportUserId { get; set; }

        [ForeignKey("SupportUserId")]
        public User? SupportUser { get; set; }
    }
}