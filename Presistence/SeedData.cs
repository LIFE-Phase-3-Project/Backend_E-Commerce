using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Presistence
{
    public static class SeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, RoleName = "Admin" },
                new Role { Id = 2, RoleName = "Employee" },
                new Role { Id = 3, RoleName = "Client" }
            );

            // Seed Categories and Subcategories
            for (int i = 1; i <= 5; i++)
            {
                modelBuilder.Entity<Category>().HasData(new Category { CategoryId = i, CategoryName = $"Category{i}" });
                modelBuilder.Entity<SubCategory>().HasData(
                    new SubCategory { SubCategoryId = i * 2 - 1, SubCategoryName = $"SubCategory{i * 2 - 1}", CategoryId = i },
                    new SubCategory { SubCategoryId = i * 2, SubCategoryName = $"SubCategory{i * 2}", CategoryId = i }
                );
            }

            // Seed Users
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                FirstName = "Admin",
                LastName = "Admin",
                Email = "admin@user.com",
                PhoneNumber = "1234567890",
                Address = "Admin Address",
                Password = BCrypt.Net.BCrypt.HashPassword("admin"),
                RoleId = 1
                });

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 2,
                FirstName = "Employee",
                LastName = "Employee",
                Email = "employee@user.com",
                PhoneNumber = "1234567890",
                Address = "Employee Address",
                Password = BCrypt.Net.BCrypt.HashPassword("employee"),
                RoleId = 2
            });


            for (int i = 3; i <= 7; i++)
            {
                modelBuilder.Entity<User>().HasData(new User
                {
                    Id = i,
                    FirstName = $"User{i}",
                    LastName = $"Last{i}",
                    Email = $"user{i}@example.com",
                    PhoneNumber = $"123456789{i}",
                    Address = $"Address {i}",
                    Password = BCrypt.Net.BCrypt.HashPassword($"pass{i}"),
                    RoleId = 3
                });
            }

            // Seed Products
            for (int i = 1; i <= 10; i++)
            {
                modelBuilder.Entity<Product>().HasData(new Product
                {
                    Id = i,
                    Title = $"Product{i}",
                    Description = $"Description for Product{i}",
                    CategoryId = i % 5 + 1, // Cycles through the 5 categories
                    SubCategoryId = i % 10 + 1, // Cycles through the 10 subcategories
                    Color = "Color" + (i % 5 + 1),
                    Image = new List<string> { $"Image{i}.jpg" },
                    Price = 100m * i,
                    Ratings = i % 5,
                    Stock = 10 * i,
                    IsDeleted = false
                });
            }
        }
    }

}
