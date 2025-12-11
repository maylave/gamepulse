using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Purchase
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int GameId { get; set; }
        public decimal Price { get; set; }
         public int Quantity { get; set; } = 1;
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("GameId")]
        public Game? Game { get; set; }
    }
}