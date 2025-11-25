using System;
using System.Collections.Generic;
using System.Text;

namespace DateAccess
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }

    
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        
        public ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();
    }
}
