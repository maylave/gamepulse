using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Game
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(5000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? OldPrice { get; set; }

        [MaxLength(50)]
        public string? Tag { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; } 

        [Required]
        public DateTime ReleaseDate { get; set; }

        public int AgeRating { get; set; }

        public bool IsPreorder { get; set; }

        [MaxLength(200)]
        public string? Developer { get; set; }

        [MaxLength(200)]
        public string? Publisher { get; set; }

        [MaxLength(500)]
        public string? ExternalUrl { get; set; } // ← Ссылка на внешний сайт/приложение (Steam, Epic и т.д.)

     
        public ICollection<СartItem> CartItems { get; } = new List<СartItem>();

       
        public ICollection<WishlistItem> WishlistItems { get; } = new List<WishlistItem>();

        
        public ICollection<Review> Reviews { get; } = new List<Review>();

        
        public ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();

       
        public ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();

        public ICollection<ViewHistory> ViewHistories { get; set; } = new List<ViewHistory>();

        
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

        
        public ICollection<GameMedia> Media { get; set; } = new List<GameMedia>();
    }
}