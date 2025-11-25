namespace WebApplication1.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GameId { get; set; }
        public int Rating { get; set; } 
        public string Text { get; set; } = string.Empty;
        public DateTime Date { get; set; }

        public User User { get; set; } = null!;
        public Game Game { get; set; } = null!;
    }
}
