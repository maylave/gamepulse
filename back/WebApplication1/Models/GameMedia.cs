using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class GameMedia
    {
        [Key]
        public int Id { get; set; }

        public int GameId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Type { get; set; } = "image"; 

        [ForeignKey("GameId")]
        public Game? Game { get; set; }
    }
}