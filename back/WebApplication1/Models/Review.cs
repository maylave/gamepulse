using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    
// Models/Review.cs
public class Review
{
    public int Id { get; set; }

    public int GameId { get; set; } 
    public Game Game { get; set; } = null!; 

    public int UserId { get; set; } 
    public User User { get; set; } = null!; 

    public int Rating { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
}
