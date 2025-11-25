using System;
using System.Collections.Generic;
using System.Text;

namespace DateAccess
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        // Внешний ключ жанра
        public int GenreId { get; set; }
        public Genre Genre { get; set; } = null!;

        // Навигационное свойство для заказов
        public ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();
    }
}
