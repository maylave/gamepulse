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
            public DbSet<ViewHistory> ViewHistories { get; set; }      
            public DbSet<GameMedia> GameMedias { get; set; }           
            public DbSet<Purchase> Purchases { get; set; }              

    
            public DbSet<ChatSession> ChatSessions { get; set; }
            public DbSet<ChatMessage> ChatMessages { get; set; }

            public ApplicationContext(DbContextOptions<ApplicationContext> options)
                : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                  base.OnModelCreating(modelBuilder);

                  
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
                        entity.Property(g => g.ExternalUrl).HasMaxLength(500);
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

                
modelBuilder.Entity<Review>(entity =>
{
    entity.HasKey(r => r.Id);

    
    entity.HasOne(r => r.Game)
          .WithMany() 
          .HasForeignKey(r => r.GameId)
          .OnDelete(DeleteBehavior.Cascade);

  
    entity.HasOne(r => r.User)
          .WithMany()
          .HasForeignKey(r => r.UserId)
          .OnDelete(DeleteBehavior.Cascade);
});
            
                  modelBuilder.Entity<Order>(entity =>
                  {
                        entity.HasKey(o => o.Id);
                        entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
                        entity.Property(o => o.Status).HasMaxLength(50);
                        entity.Property(o => o.OrderDate)
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                        entity.HasOne(o => o.User)
                        .WithMany(u => u.Orders)
                        .HasForeignKey(o => o.UserId)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                 
                  modelBuilder.Entity<Genre>(entity =>
                  {
                        entity.HasKey(g => g.Id);
                        entity.Property(g => g.Name).IsRequired().HasMaxLength(100);
                  });

                 
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


                  modelBuilder.Entity<ViewHistory>(entity =>
                  {
                        entity.HasKey(vh => vh.Id);

                        entity.HasOne(vh => vh.User)
                        .WithMany(u => u.ViewHistories)
                        .HasForeignKey(vh => vh.UserId)
                        .OnDelete(DeleteBehavior.Cascade);

                        entity.HasOne(vh => vh.Game)
                        .WithMany(g => g.ViewHistories)
                        .HasForeignKey(vh => vh.GameId)
                        .OnDelete(DeleteBehavior.Cascade);

                        entity.Property(vh => vh.ViewedAt)
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");
                  });

                
                  modelBuilder.Entity<GameMedia>(entity =>
                  {
                        entity.HasKey(gm => gm.Id);
                        entity.Property(gm => gm.Url).IsRequired().HasMaxLength(500);
                        entity.Property(gm => gm.Type).IsRequired().HasMaxLength(20); // "image" или "video"

                        entity.HasOne(gm => gm.Game)
                        .WithMany(g => g.Media)
                        .HasForeignKey(gm => gm.GameId)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                  
                  modelBuilder.Entity<Purchase>(entity =>
                  {
                        entity.HasKey(p => p.Id);
                        entity.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();
                        entity.Property(p => p.PurchasedAt)
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                        entity.HasOne(p => p.User)
                        .WithMany(u => u.Purchases)
                        .HasForeignKey(p => p.UserId)
                        .OnDelete(DeleteBehavior.Cascade);

                        entity.HasOne(p => p.Game)
                        .WithMany(g => g.Purchases)
                        .HasForeignKey(p => p.GameId)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                  // === ChatSession ===
                  modelBuilder.Entity<ChatSession>(entity =>
                  {
                        entity.HasKey(s => s.Id);
                        entity.Property(s => s.ClientName).IsRequired().HasMaxLength(200);
                        entity.Property(s => s.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                        entity.Property(s => s.LastActivity).HasDefaultValueSql("CURRENT_TIMESTAMP");

                        entity.HasOne(s => s.Client)
                        .WithMany(u => u.ChatSessions)
                        .HasForeignKey(s => s.ClientId)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                  // === ChatMessage ===
                  modelBuilder.Entity<ChatMessage>(entity =>
                  {
                        entity.HasKey(m => m.Id);
                        entity.Property(m => m.Content).IsRequired().HasMaxLength(4000);
                        entity.Property(m => m.SentAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                        entity.HasOne(m => m.ChatSession)
                        .WithMany(cs => cs.Messages)
                        .HasForeignKey(m => m.ChatSessionId)
                        .OnDelete(DeleteBehavior.Cascade);

                        entity.HasOne(m => m.SupportUser)
                        .WithMany()
                        .HasForeignKey(m => m.SupportUserId)
                        .OnDelete(DeleteBehavior.SetNull);
                  });
            }
      }
}