// DateAccess/ApplicationContext.cs
using Microsoft.EntityFrameworkCore;

namespace DateAccess
{
    public class ApplicationContext : DbContext
    {
       
         public ApplicationContext() { }

       
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        // DbSet'ы
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // ⚠️ Если вы используете DI (как в ASP.NET Core), этот блок НЕ должен выполняться!
            // Он нужен только если вы создаете контекст вручную (например, в Program.cs).
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    @"Server=(localdb)\MSSQLLocalDB;Database=GameStoreDb;Trusted_Connection=True;Encrypt=False;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // === User ===
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Age).IsRequired();
            });

            // === Genre ===
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Name).IsRequired().HasMaxLength(50);
            });

            // === Game ===
            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Title).IsRequired().HasMaxLength(200);
                entity.Property(g => g.Description).HasMaxLength(2000);
                entity.Property(g => g.Price).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(g => g.ImageUrl).HasMaxLength(500);
                entity.Property(g => g.ReleaseDate).IsRequired();

                // Отношение: Game → Genre
                entity.HasOne(g => g.Genre)
                      .WithMany(genre => genre.Games)
                      .HasForeignKey(g => g.GenreId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // === Order ===
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.OrderDate).IsRequired();
                entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();

                // Отношение: Order → User
                entity.HasOne(o => o.User)
                      .WithMany(u => u.Orders)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // === OrderItem ===
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.Id);
                entity.Property(oi => oi.Quantity).IsRequired();
                entity.Property(oi => oi.Price).HasColumnType("decimal(18,2)").IsRequired();

                // Отношение: OrderItem → Order
                entity.HasOne(oi => oi.Order)
                      .WithMany(o => o.OrderItems)
                      .HasForeignKey(oi => oi.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Отношение: OrderItem → Game
                entity.HasOne(oi => oi.Game)
                      .WithMany(g => g.OrderItems)
                      .HasForeignKey(oi => oi.GameId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}