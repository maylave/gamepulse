using System;
using System.Collections.Generic;
using System.Text;

namespace DateAccess
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; } 

       
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int GameId { get; set; }
        public Game Game { get; set; } = null!;
    }
}
