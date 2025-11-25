// WebApplication1/Models/Order.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

     
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; 

        // Связь с пользователем
        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

       
        public ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();
    }
}