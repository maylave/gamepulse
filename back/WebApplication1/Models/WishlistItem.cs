namespace WebApplication1.Models
{
    public class WishlistItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public int GameId { get; set; }

        public User User { get; set; } = null;

        public Game Game { get; set; } = null;
    }
}
