using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
namespace DateAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new ApplicationContext())
            {
                db.Database.EnsureCreated();
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Интернет-магазин игр ===");
                Console.WriteLine("1. Пользователи");
                Console.WriteLine("2. Жанры");
                Console.WriteLine("3. Игры");
                Console.WriteLine("4. Заказы");
                Console.WriteLine("0. Выход");
                Console.Write("\nВыберите раздел: ");

                var input = Console.ReadLine();
                if (input == "0") break;

                switch (input)
                {
                    case "1": ManageUsers(); break;
                    case "2": ManageGenres(); break;
                    case "3": ManageGames(); break;
                    case "4": ManageOrders(); break;
                    default:
                        Console.WriteLine("Неверный выбор. Нажмите любую клавишу...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        
        static void ManageUsers()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Пользователи ===");
                Console.WriteLine("1. Показать всех");
                Console.WriteLine("2. Добавить");
                Console.WriteLine("3. Удалить");
                Console.WriteLine("4. Изменить");
                Console.WriteLine("0. Назад");
                Console.Write("Выбор: ");

                var input = Console.ReadLine();
                if (input == "0") return;

                switch (input)
                {
                    case "1": ShowAll<User>(u => $"{u.Id}. {u.Name} ({u.Email}) — {u.Age} лет"); break;
                    case "2": AddUser(); break;
                    case "3": DeleteById<User>(); break;
                    case "4": UpdateUser(); break;
                    default: Console.WriteLine("Неверно"); Console.ReadKey(); break;
                }
            }
        }

        static void AddUser()
        {
            Console.Write("Имя: ");
            var name = Console.ReadLine() ?? "";
            Console.Write("Email: ");
            var email = Console.ReadLine() ?? "";
            Console.Write("Пароль: ");
            var password = Console.ReadLine() ?? "";
            Console.Write("Возраст: ");
            if (!int.TryParse(Console.ReadLine(), out int age)) { Console.WriteLine("Ошибка возраста"); Console.ReadKey(); return; }

            using var db = new ApplicationContext();
            db.Users.Add(new User { Name = name, Email = email, PasswordHash = BCrypt.Net.BCrypt.HashPassword(password), Age = age });
            db.SaveChanges();
            Console.WriteLine("Пользователь добавлен.");
            Console.ReadKey();
        }

        static void UpdateUser()
        {
            Console.Write("ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;

            using var db = new ApplicationContext();
            var user = db.Users.Find(id);
            if (user == null) { Console.WriteLine("Не найден"); Console.ReadKey(); return; }

            Console.Write($"Имя [{user.Name}]: ");
            var name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name)) user.Name = name;

            Console.Write($"Email [{user.Email}]: ");
            var email = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email)) user.Email = email;

            Console.Write($"Пароль [****]: ");
            var password = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(password)) user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            Console.Write($"Возраст [{user.Age}]: ");
            var ageInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(ageInput) && int.TryParse(ageInput, out int age))
                user.Age = age;

            db.SaveChanges();
            Console.WriteLine("Обновлено.");
            Console.ReadKey();
        }

        
        static void ManageGenres()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Жанры ===");
                Console.WriteLine("1. Показать все");
                Console.WriteLine("2. Добавить");
                Console.WriteLine("3. Удалить");
                Console.WriteLine("0. Назад");
                Console.Write("Выбор: ");

                var input = Console.ReadLine();
                if (input == "0") return;

                switch (input)
                {
                    case "1": ShowAll<Genre>(g => $"{g.Id}. {g.Name}"); break;
                    case "2": AddGenre(); break;
                    case "3": DeleteById<Genre>(); break;
                    default: Console.WriteLine("Неверно"); Console.ReadKey(); break;
                }
            }
        }

        static void AddGenre()
        {
            Console.Write("Название жанра: ");
            var name = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Пустое имя"); Console.ReadKey(); return; }

            using var db = new ApplicationContext();
            db.Genres.Add(new Genre { Name = name });
            db.SaveChanges();
            Console.WriteLine("Жанр добавлен.");
            Console.ReadKey();
        }

        
        static void ManageGames()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Игры ===");
                Console.WriteLine("1. Показать все");
                Console.WriteLine("2. Добавить");
                Console.WriteLine("3. Удалить");
                Console.WriteLine("4. Изменить");
                Console.WriteLine("0. Назад");
                Console.Write("Выбор: ");

                var input = Console.ReadLine();
                if (input == "0") return;

                switch (input)
                {
                    case "1": ShowGames(); break;
                    case "2": AddGame(); break;
                    case "3": DeleteById<Game>(); break;
                    case "4": UpdateGame(); break;
                    default: Console.WriteLine("Неверно"); Console.ReadKey(); break;
                }
            }
        }

        static void ShowGames()
        {
            using var db = new ApplicationContext();
            var games = db.Games.Include(g => g.Genre).ToList();
            Console.WriteLine("\n--- Игры ---");
            foreach (var g in games)
            {
                Console.WriteLine($"{g.Id}. {g.Title} — {g.Genre?.Name ?? "Без жанра"} | {g.Price:C}");
            }
            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }

        static void AddGame()
        {
            Console.Write("Название: ");
            var title = Console.ReadLine() ?? "";
            Console.Write("Описание: ");
            var desc = Console.ReadLine() ?? "";
            Console.Write("Цена: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price)) return;
            Console.Write("Дата релиза (ГГГГ-ММ-ДД): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime date)) return;
            Console.Write("URL изображения: ");
            var img = Console.ReadLine() ?? "";

            ShowAll<Genre>(g => $"{g.Id}. {g.Name}");
            Console.Write("ID жанра: ");
            if (!int.TryParse(Console.ReadLine(), out int genreId)) return;

            using var db = new ApplicationContext();
            if (db.Genres.Find(genreId) == null) { Console.WriteLine("Жанр не найден"); Console.ReadKey(); return; }

            db.Games.Add(new Game
            {
                Title = title,
                Description = desc,
                Price = price,
                ReleaseDate = date,
                ImageUrl = img,
                GenreId = genreId
            });
            db.SaveChanges();
            Console.WriteLine("Игра добавлена.");
            Console.ReadKey();
        }

        static void UpdateGame()
        {
            Console.Write("ID игры: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;

            using var db = new ApplicationContext();
            var game = db.Games.Include(g => g.Genre).FirstOrDefault(g => g.Id == id);
            if (game == null) { Console.WriteLine("Не найдена"); Console.ReadKey(); return; }

            Console.Write($"Название [{game.Title}]: ");
            var title = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(title)) game.Title = title;

            Console.Write($"Цена [{game.Price:C}]: ");
            var priceInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(priceInput) && decimal.TryParse(priceInput, out decimal price))
                game.Price = price;

            Console.Write($"Жанр [{game.Genre?.Name}] — новый ID: ");
            var genreInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(genreInput) && int.TryParse(genreInput, out int gid))
            {
                if (db.Genres.Find(gid) != null) game.GenreId = gid;
            }

            db.SaveChanges();
            Console.WriteLine("Обновлено.");
            Console.ReadKey();
        }

       
        static void ManageOrders()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Заказы ===");
                Console.WriteLine("1. Показать все");
                Console.WriteLine("2. Добавить");
                Console.WriteLine("0. Назад");
                Console.Write("Выбор: ");

                var input = Console.ReadLine();
                if (input == "0") return;

                switch (input)
                {
                    case "1": ShowOrders(); break;
                    case "2": AddOrder(); break;
                    default: Console.WriteLine("Неверно"); Console.ReadKey(); break;
                }
            }
        }

        static void ShowOrders()
        {
            using var db = new ApplicationContext();
            var orders = db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Game)
                .ToList();

            foreach (var o in orders)
            {
                Console.WriteLine($"\nЗаказ #{o.Id} от {o.User?.Name} ({o.OrderDate:dd.MM.yyyy}) — {o.TotalAmount:C}");
                foreach (var i in o.OrderItems)
                {
                    Console.WriteLine($"  • {i.Game?.Title} x{i.Quantity}");
                }
            }
            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }

        static void AddOrder()
        {
            ShowAll<User>(u => $"{u.Id}. {u.Name}");
            Console.Write("ID пользователя: ");
            if (!int.TryParse(Console.ReadLine(), out int userId)) return;

            using var db = new ApplicationContext();
            var user = db.Users.Find(userId);
            if (user == null) { Console.WriteLine("Пользователь не найден"); Console.ReadKey(); return; }

            var order = new Order { UserId = userId, OrderDate = DateTime.Now, TotalAmount = 0 };
            db.Orders.Add(order);
            db.SaveChanges();

            decimal total = 0;
            while (true)
            {
                ShowAll<Game>(g => $"{g.Id}. {g.Title} — {g.Price:C}");
                Console.Write("ID игры (0 — завершить): ");
                if (!int.TryParse(Console.ReadLine(), out int gameId) || gameId == 0) break;

                var game = db.Games.Find(gameId);
                if (game == null) continue;

                Console.Write("Количество: ");
                if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0) continue;

                db.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    GameId = gameId,
                    Quantity = qty,
                    Price = game.Price
                });
                total += game.Price * qty;
            }

            order.TotalAmount = total;
            db.SaveChanges();
            Console.WriteLine($"Заказ #{order.Id} создан. Сумма: {total:C}");
            Console.ReadKey();
        }

        // ========== УНИВЕРСАЛЬНЫЕ МЕТОДЫ ==========
        static void ShowAll<T>(Func<T, string> formatter) where T : class
        {
            using var db = new ApplicationContext();
            var set = db.Set<T>().ToList();
            Console.WriteLine($"\n--- {typeof(T).Name} ---");
            if (!set.Any()) Console.WriteLine("Нет записей.");
            else set.ForEach(item => Console.WriteLine(formatter(item)));
            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }

        static void DeleteById<T>() where T : class
        {
            Console.Write($"ID {typeof(T).Name}: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;

            using var db = new ApplicationContext();
            var entity = db.Set<T>().Find(id);
            if (entity == null) { Console.WriteLine("Не найдено"); Console.ReadKey(); return; }

            db.Set<T>().Remove(entity);
            db.SaveChanges();
            Console.WriteLine("Удалено.");
            Console.ReadKey();
        }
    }
}