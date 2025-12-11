
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
    public class User : IdentityUser<int> // int вместо string Id
    {
        
       
    
       
        public string Name { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; } 


        public string? EmailConfirmationCode { get; set; } 
        public override bool EmailConfirmed { get; set; }

        
        public ICollection<ViewHistory> ViewHistories { get; set; } = new List<ViewHistory>();
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
        public ICollection<СartItem> CartItems { get; set; } = new List<СartItem>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
