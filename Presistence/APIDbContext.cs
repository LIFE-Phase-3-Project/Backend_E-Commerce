using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
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

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureUser(modelBuilder);
            ConfigureProduct(modelBuilder);
            ConfigureCategory(modelBuilder);
            ConfigureSubCategory(modelBuilder);
            ConfigureOrderDetail(modelBuilder);
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

                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict); // Specify delete behavior
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Provide your connection string or other configurations here
            }
        }
    }
}
