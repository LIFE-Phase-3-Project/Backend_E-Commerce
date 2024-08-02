using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Presistence
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<WishlistEntry> WishlistEntries { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Discount> Discounts { get; set; }

        public DbSet<UserAddress> UserAddresses { get; set; }

        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatSession> ChatSessions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureUser(modelBuilder);
            ConfigureProduct(modelBuilder);
            ConfigureCategory(modelBuilder);
            ConfigureSubCategory(modelBuilder);
            ConfigureWishlistEntry(modelBuilder);
            ConfigureOrderDetail(modelBuilder);
            ConfigurePayment(modelBuilder);
            ConfigureChatSession(modelBuilder);
        }

        private void ConfigureWishlistEntry(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WishlistEntry>().HasKey(w => new { w.UserId, w.ProductId });

        }
        private void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserRole)
                .WithMany()
                .HasForeignKey(u => u.RoleId);
        }

        private void ConfigureProduct(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.HasQueryFilter(p => !p.IsDeleted);
                entity.HasOne(p => p.SubCategory)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.SubCategoryId)
                      .OnDelete(DeleteBehavior.Restrict); 
            });
        }

        private void ConfigureCategory(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Specify delete behavior
        }


        private void ConfigureSubCategory(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubCategory>()
                .HasOne(sc => sc.Category)
                .WithMany(c => c.Subcategories)
                .HasForeignKey(sc => sc.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Specify delete behavior

            modelBuilder.Entity<Product>()
                .HasOne(p => p.SubCategory)
                .WithMany(sc => sc.Products)
                .HasForeignKey(p => p.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Specify delete behavior
        }

        private void ConfigureOrderDetail(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.Price).HasPrecision(18, 2);

                entity.HasOne(od => od.OrderData)
                      .WithMany()
                      .HasForeignKey(od => od.OrderId);

                entity.HasOne(od => od.Product)
                      .WithMany()
                      .HasForeignKey(od => od.ProductId);

                entity.HasOne(od => od.User)
                      .WithMany()
                      .HasForeignKey(od => od.UserId);
            });
        }

        private void ConfigurePayment(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasOne(p => p.Order)
                      .WithOne(o => o.Payment)
                      .HasForeignKey<Payment>(p => p.OrderId);
            });
        }

        private void ConfigureChatSession(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatSession>(entity =>
            {
                entity.HasKey(cs => cs.Id);
                entity.Property(cs => cs.Status).HasMaxLength(20);

                // No navigation property on ChatSession
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(cm => cm.Id);
                entity.Property(cm => cm.Sender).IsRequired().HasMaxLength(100);
                entity.Property(cm => cm.Recipient).IsRequired().HasMaxLength(100);
                entity.Property(cm => cm.Message).IsRequired();
                entity.Property(cm => cm.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configuring the foreign key relationship
                entity.HasOne(cm => cm.ChatSession)
                      .WithMany() // No navigation property on ChatSession
                      .HasForeignKey(cm => cm.SessionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Provide your connection string or other configurations here
            }
        }
    }
}
