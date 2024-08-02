using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presistence.Migrations
{
    /// <inheritdoc />
    public partial class chatTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.CreateTable(
                name: "ChatSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSessions", x => x.Id);
                });

           

          

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sender = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Recipient = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    SessionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "ChatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });




            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SessionId",
                table: "ChatMessages",
                column: "SessionId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropTable(
                name: "ChatSessions");

        }
    }
}
