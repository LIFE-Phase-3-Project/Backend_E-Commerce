﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Presistence;

#nullable disable

namespace Presistence.Migrations
{
    [DbContext(typeof(APIDbContext))]
    partial class APIDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.CartItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("ShoppingCartId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("ShoppingCartId");

                    b.ToTable("CartItems");
                });

            modelBuilder.Entity("Domain.Entities.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"));

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CategoryId");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            CategoryId = 1,
                            CategoryName = "Category1"
                        },
                        new
                        {
                            CategoryId = 2,
                            CategoryName = "Category2"
                        },
                        new
                        {
                            CategoryId = 3,
                            CategoryName = "Category3"
                        },
                        new
                        {
                            CategoryId = 4,
                            CategoryName = "Category4"
                        },
                        new
                        {
                            CategoryId = 5,
                            CategoryName = "Category5"
                        });
                });

            modelBuilder.Entity("Domain.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<decimal>("Price")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Ratings")
                        .HasColumnType("int");

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.Property<int>("SubCategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("SubCategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CategoryId = 2,
                            Color = "Color2",
                            Description = "Description for Product1",
                            Image = "[\"Image1.jpg\"]",
                            IsDeleted = false,
                            Price = 100m,
                            Ratings = 1,
                            Stock = 10,
                            SubCategoryId = 2,
                            Title = "Product1"
                        },
                        new
                        {
                            Id = 2,
                            CategoryId = 3,
                            Color = "Color3",
                            Description = "Description for Product2",
                            Image = "[\"Image2.jpg\"]",
                            IsDeleted = false,
                            Price = 200m,
                            Ratings = 2,
                            Stock = 20,
                            SubCategoryId = 3,
                            Title = "Product2"
                        },
                        new
                        {
                            Id = 3,
                            CategoryId = 4,
                            Color = "Color4",
                            Description = "Description for Product3",
                            Image = "[\"Image3.jpg\"]",
                            IsDeleted = false,
                            Price = 300m,
                            Ratings = 3,
                            Stock = 30,
                            SubCategoryId = 4,
                            Title = "Product3"
                        },
                        new
                        {
                            Id = 4,
                            CategoryId = 5,
                            Color = "Color5",
                            Description = "Description for Product4",
                            Image = "[\"Image4.jpg\"]",
                            IsDeleted = false,
                            Price = 400m,
                            Ratings = 4,
                            Stock = 40,
                            SubCategoryId = 5,
                            Title = "Product4"
                        },
                        new
                        {
                            Id = 5,
                            CategoryId = 1,
                            Color = "Color1",
                            Description = "Description for Product5",
                            Image = "[\"Image5.jpg\"]",
                            IsDeleted = false,
                            Price = 500m,
                            Ratings = 0,
                            Stock = 50,
                            SubCategoryId = 6,
                            Title = "Product5"
                        },
                        new
                        {
                            Id = 6,
                            CategoryId = 2,
                            Color = "Color2",
                            Description = "Description for Product6",
                            Image = "[\"Image6.jpg\"]",
                            IsDeleted = false,
                            Price = 600m,
                            Ratings = 1,
                            Stock = 60,
                            SubCategoryId = 7,
                            Title = "Product6"
                        },
                        new
                        {
                            Id = 7,
                            CategoryId = 3,
                            Color = "Color3",
                            Description = "Description for Product7",
                            Image = "[\"Image7.jpg\"]",
                            IsDeleted = false,
                            Price = 700m,
                            Ratings = 2,
                            Stock = 70,
                            SubCategoryId = 8,
                            Title = "Product7"
                        },
                        new
                        {
                            Id = 8,
                            CategoryId = 4,
                            Color = "Color4",
                            Description = "Description for Product8",
                            Image = "[\"Image8.jpg\"]",
                            IsDeleted = false,
                            Price = 800m,
                            Ratings = 3,
                            Stock = 80,
                            SubCategoryId = 9,
                            Title = "Product8"
                        },
                        new
                        {
                            Id = 9,
                            CategoryId = 5,
                            Color = "Color5",
                            Description = "Description for Product9",
                            Image = "[\"Image9.jpg\"]",
                            IsDeleted = false,
                            Price = 900m,
                            Ratings = 4,
                            Stock = 90,
                            SubCategoryId = 10,
                            Title = "Product9"
                        },
                        new
                        {
                            Id = 10,
                            CategoryId = 1,
                            Color = "Color1",
                            Description = "Description for Product10",
                            Image = "[\"Image10.jpg\"]",
                            IsDeleted = false,
                            Price = 1000m,
                            Ratings = 0,
                            Stock = 100,
                            SubCategoryId = 1,
                            Title = "Product10"
                        });
                });

            modelBuilder.Entity("Domain.Entities.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("Domain.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            RoleName = "Admin"
                        },
                        new
                        {
                            Id = 2,
                            RoleName = "Employee"
                        },
                        new
                        {
                            Id = 3,
                            RoleName = "Client"
                        });
                });

            modelBuilder.Entity("Domain.Entities.ShoppingCart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CartIdentifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("ShoppingCarts");
                });

            modelBuilder.Entity("Domain.Entities.SubCategory", b =>
                {
                    b.Property<int>("SubCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SubCategoryId"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("SubCategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SubCategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("SubCategories");

                    b.HasData(
                        new
                        {
                            SubCategoryId = 1,
                            CategoryId = 1,
                            SubCategoryName = "SubCategory1"
                        },
                        new
                        {
                            SubCategoryId = 2,
                            CategoryId = 1,
                            SubCategoryName = "SubCategory2"
                        },
                        new
                        {
                            SubCategoryId = 3,
                            CategoryId = 2,
                            SubCategoryName = "SubCategory3"
                        },
                        new
                        {
                            SubCategoryId = 4,
                            CategoryId = 2,
                            SubCategoryName = "SubCategory4"
                        },
                        new
                        {
                            SubCategoryId = 5,
                            CategoryId = 3,
                            SubCategoryName = "SubCategory5"
                        },
                        new
                        {
                            SubCategoryId = 6,
                            CategoryId = 3,
                            SubCategoryName = "SubCategory6"
                        },
                        new
                        {
                            SubCategoryId = 7,
                            CategoryId = 4,
                            SubCategoryName = "SubCategory7"
                        },
                        new
                        {
                            SubCategoryId = 8,
                            CategoryId = 4,
                            SubCategoryName = "SubCategory8"
                        },
                        new
                        {
                            SubCategoryId = 9,
                            CategoryId = 5,
                            SubCategoryName = "SubCategory9"
                        },
                        new
                        {
                            SubCategoryId = 10,
                            CategoryId = 5,
                            SubCategoryName = "SubCategory10"
                        });
                });

            modelBuilder.Entity("Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Address = "Admin Address",
                            Email = "admin@user.com",
                            FirstName = "Admin",
                            LastName = "Admin",
                            Password = "$2a$10$O5YzMHshCXXJ9NeUEcFbbufLi7cBK3vs.1N/hkkLG6XY2MCidrCcm",
                            PhoneNumber = "1234567890",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 2,
                            Address = "Employee Address",
                            Email = "employee@user.com",
                            FirstName = "Employee",
                            LastName = "Employee",
                            Password = "$2a$10$ezb1pPRjQPsbrcBNQjniWuVBxAzDTyG6ZvL8uSIGHt4CJipk/WCbS",
                            PhoneNumber = "1234567890",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 3,
                            Address = "Address 3",
                            Email = "user3@example.com",
                            FirstName = "User3",
                            LastName = "Last3",
                            Password = "$2a$10$fSO9qH.5nOpgFI8mTw3V/uRxFqH6qqaSRzu.cjuBcvbTJPrIROb.6",
                            PhoneNumber = "1234567893",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 4,
                            Address = "Address 4",
                            Email = "user4@example.com",
                            FirstName = "User4",
                            LastName = "Last4",
                            Password = "$2a$10$aihCPqeLpJVJWtrLUTqMKO4jMhTy1XQOIxb5ZwUPMCBqj4IeV2Cqy",
                            PhoneNumber = "1234567894",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 5,
                            Address = "Address 5",
                            Email = "user5@example.com",
                            FirstName = "User5",
                            LastName = "Last5",
                            Password = "$2a$10$JQN6tlklQnrP0MRQGJno8uC7EzudpI4ZYjV77JjRzlvTGiFv2d9em",
                            PhoneNumber = "1234567895",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 6,
                            Address = "Address 6",
                            Email = "user6@example.com",
                            FirstName = "User6",
                            LastName = "Last6",
                            Password = "$2a$10$c1dPmSQGSTnQfX9cR9u2b.j9VNpiz7acQ06HJSrn8b5a7/RBdKSUC",
                            PhoneNumber = "1234567896",
                            RoleId = 3
                        },
                        new
                        {
                            Id = 7,
                            Address = "Address 7",
                            Email = "user7@example.com",
                            FirstName = "User7",
                            LastName = "Last7",
                            Password = "$2a$10$Ywa4r5dws1x9H2JMoqDf2uZB5AngY7NxlXKCiBASLVhILLoUFGEzS",
                            PhoneNumber = "1234567897",
                            RoleId = 3
                        });
                });

            modelBuilder.Entity("Domain.Entities.WishlistEntry", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("WishlistEntries");
                });

            modelBuilder.Entity("Domain.Entities.CartItem", b =>
                {
                    b.HasOne("Domain.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.ShoppingCart", null)
                        .WithMany("CartItems")
                        .HasForeignKey("ShoppingCartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Domain.Entities.Product", b =>
                {
                    b.HasOne("Domain.Entities.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Entities.SubCategory", "SubCategory")
                        .WithMany("Products")
                        .HasForeignKey("SubCategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("SubCategory");
                });

            modelBuilder.Entity("Domain.Entities.Review", b =>
                {
                    b.HasOne("Domain.Entities.Product", "Product")
                        .WithMany("Reviews")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.SubCategory", b =>
                {
                    b.HasOne("Domain.Entities.Category", "Category")
                        .WithMany("Subcategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Domain.Entities.User", b =>
                {
                    b.HasOne("Domain.Entities.Role", "UserRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserRole");
                });

            modelBuilder.Entity("Domain.Entities.WishlistEntry", b =>
                {
                    b.HasOne("Domain.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.Category", b =>
                {
                    b.Navigation("Products");

                    b.Navigation("Subcategories");
                });

            modelBuilder.Entity("Domain.Entities.Product", b =>
                {
                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("Domain.Entities.ShoppingCart", b =>
                {
                    b.Navigation("CartItems");
                });

            modelBuilder.Entity("Domain.Entities.SubCategory", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
