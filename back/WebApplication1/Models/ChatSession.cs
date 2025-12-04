using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class ChatSession
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string ClientName { get; set; } = string.Empty;

        [Required]
        public int ClientId { get; set; } // Связь с User.Id

        [ForeignKey("ClientId")]
        public User Client { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}