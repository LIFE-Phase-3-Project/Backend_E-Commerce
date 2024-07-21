using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Presistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName" },
                values: new object[,]
                {
                    { 1, "Category1" },
                    { 2, "Category2" },
                    { 3, "Category3" },
                    { 4, "Category4" },
                    { 5, "Category5" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "RoleName" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Employee" },
                    { 3, "Client" }
                });

            migrationBuilder.InsertData(
                table: "SubCategories",
                columns: new[] { "SubCategoryId", "CategoryId", "SubCategoryName" },
                values: new object[,]
                {
                    { 1, 1, "SubCategory1" },
                    { 2, 1, "SubCategory2" },
                    { 3, 2, "SubCategory3" },
                    { 4, 2, "SubCategory4" },
                    { 5, 3, "SubCategory5" },
                    { 6, 3, "SubCategory6" },
                    { 7, 4, "SubCategory7" },
                    { 8, 4, "SubCategory8" },
                    { 9, 5, "SubCategory9" },
                    { 10, 5, "SubCategory10" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "Email", "FirstName", "LastName", "Password", "PhoneNumber", "RoleId" },
                values: new object[,]
                {
                    { 1, "Admin Address", "admin@user.com", "Admin", "Admin", "admin", "1234567890", 1 },
                    { 2, "Employee Address", "employee@user.com", "Employee", "Employee", "employee", "1234567890", 2 },
                    { 3, "Address 3", "user3@example.com", "User3", "Last3", "pass3", "1234567893", 3 },
                    { 4, "Address 4", "user4@example.com", "User4", "Last4", "pass4", "1234567894", 3 },
                    { 5, "Address 5", "user5@example.com", "User5", "Last5", "pass5", "1234567895", 3 },
                    { 6, "Address 6", "user6@example.com", "User6", "Last6", "pass6", "1234567896", 3 },
                    { 7, "Address 7", "user7@example.com", "User7", "Last7", "pass7", "1234567897", 3 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Color", "Description", "Image", "IsDeleted", "Price", "Ratings", "Stock", "SubCategoryId", "Title" },
                values: new object[,]
                {
                    { 1, 2, "Color2", "Description for Product1", "[\"Image1.jpg\"]", false, 100m, 1, 10, 2, "Product1" },
                    { 2, 3, "Color3", "Description for Product2", "[\"Image2.jpg\"]", false, 200m, 2, 20, 3, "Product2" },
                    { 3, 4, "Color4", "Description for Product3", "[\"Image3.jpg\"]", false, 300m, 3, 30, 4, "Product3" },
                    { 4, 5, "Color5", "Description for Product4", "[\"Image4.jpg\"]", false, 400m, 4, 40, 5, "Product4" },
                    { 5, 1, "Color1", "Description for Product5", "[\"Image5.jpg\"]", false, 500m, 0, 50, 6, "Product5" },
                    { 6, 2, "Color2", "Description for Product6", "[\"Image6.jpg\"]", false, 600m, 1, 60, 7, "Product6" },
                    { 7, 3, "Color3", "Description for Product7", "[\"Image7.jpg\"]", false, 700m, 2, 70, 8, "Product7" },
                    { 8, 4, "Color4", "Description for Product8", "[\"Image8.jpg\"]", false, 800m, 3, 80, 9, "Product8" },
                    { 9, 5, "Color5", "Description for Product9", "[\"Image9.jpg\"]", false, 900m, 4, 90, 10, "Product9" },
                    { 10, 1, "Color1", "Description for Product10", "[\"Image10.jpg\"]", false, 1000m, 0, 100, 1, "Product10" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 5);
        }
    }
}
