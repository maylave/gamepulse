

namespace WebApplication1.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string? Tag { get; set; }
        public string? ImageUrl { get; set; }
        public string? Category { get; set; } // "top", "upcoming", "free"
        public DateTime ReleaseDate { get; set; }
        public int AgeRating { get; set; }
        public bool IsPreorder { get; set; }
        public string? Developer { get; set; }
        public string? Publisher { get; set; }


        // Навигация
       
      

        
      
        public ICollection<СartItem> CartItems { get; } = new List<СartItem>();
        public ICollection<WishlistItem> WishlistItems { get; } = new List<WishlistItem>();
        public ICollection<Review> Reviews { get; } = new List<Review>();
        public ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();
        public ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>(); // ✅ Only this!
    }
}