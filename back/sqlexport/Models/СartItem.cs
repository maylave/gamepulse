namespace WebApplication1.Models
{
    public class СartItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public int GameId {  get; set; }
        public int Quantity { get; set; }
        public User User { get; set; } = null;

        public Game Game { get; set; } = null;


    }
}
