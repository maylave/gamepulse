
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
    public class User : IdentityUser<int> // int вместо string Id
    {
        
       
    
       
        public string Name { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; } 


        public string? EmailConfirmationCode { get; set; } // Новое поле
        public override bool EmailConfirmed { get; set; }

       
        public ICollection<СartItem> CartItems { get; } = new List<СartItem>();
        public ICollection<WishlistItem> WishlistItems { get; } = new List<WishlistItem>();
        public ICollection<Review> Reviews { get; } = new List<Review>();
        public ICollection<Order> Orders { get; } = new List<Order>();
        public ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
    }
}
