using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;

namespace WebApplication1
{
    public class ApplicationContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<СartItem> CartItems { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<GameGenre> GameGenres { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === User ===
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.AvatarUrl).IsRequired().HasMaxLength(500)
                       .HasDefaultValue("/images/defaults/avatar.png");
            });

            // === Game ===
            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Title).IsRequired().HasMaxLength(256);
                entity.Property(g => g.Description).HasMaxLength(5000);
                entity.Property(g => g.Price).HasColumnType("decimal(18,2)").IsRequired();
              
                entity.Property(g => g.Developer).HasMaxLength(200);

              
            });

            // === CartItem ===
            modelBuilder.Entity<СartItem>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Quantity).IsRequired();

                entity.HasOne(c => c.User)
                      .WithMany(u => u.CartItems)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Game)
                      .WithMany(g => g.CartItems)
                      .HasForeignKey(c => c.GameId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // === WishlistItem ===
            modelBuilder.Entity<WishlistItem>(entity =>
            {
                entity.HasKey(w => w.Id);

                entity.HasOne(w => w.User)
                      .WithMany(u => u.WishlistItems)
                      .HasForeignKey(w => w.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(w => w.Game)
                      .WithMany(g => g.WishlistItems)
                      .HasForeignKey(w => w.GameId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // === Review ===
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Rating).IsRequired();
                entity.Property(r => r.Text).IsRequired().HasMaxLength(2000);
                entity.Property(r => r.Date).IsRequired();

                entity.HasOne(r => r.User)
                      .WithMany(u => u.Reviews)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Game)
                      .WithMany(g => g.Reviews)
                      .HasForeignKey(r => r.GameId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // === Order ===
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(o => o.Status).HasMaxLength(50);
                entity.Property(o => o.OrderDate).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(o => o.User)
                      .WithMany(u => u.Orders)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // === Genre ===
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Name).IsRequired().HasMaxLength(100);
            });

            // === GameGenre (Join Entity for Many-to-Many) ===
            modelBuilder.Entity<GameGenre>(entity =>
            {
                entity.HasKey(gg => new { gg.GameId, gg.GenreId });

                entity.HasOne(gg => gg.Game)
                      .WithMany(g => g.GameGenres)
                      .HasForeignKey(gg => gg.GameId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(gg => gg.Genre)
                      .WithMany(g => g.GameGenres)
                      .HasForeignKey(gg => gg.GenreId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}