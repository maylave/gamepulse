using System;
using System.Collections.Generic;
using System.Text;
using WebApplication1.Models;
namespace WebApplication1
{

    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();
    }
}
