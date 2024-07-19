using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presistence.Migrations
{
    /// <inheritdoc />
    public partial class Wishlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WishlistEntries_UserId_ProductId",
                table: "WishlistEntries");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WishlistEntries",
                table: "WishlistEntries",
                columns: new[] { "UserId", "ProductId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WishlistEntries",
                table: "WishlistEntries");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistEntries_UserId_ProductId",
                table: "WishlistEntries",
                columns: new[] { "UserId", "ProductId" },
                unique: true);
        }
    }
}
