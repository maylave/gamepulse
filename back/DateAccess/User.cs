using System;

namespace DateAccess
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // ← хэш, не пароль!
        public int Age { get; set; }
    

        // Навигационное свойство: один пользователь → много заказов
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}