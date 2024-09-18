using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TypicalTechTools.Migrations
{
    /// <inheritdoc />
    public partial class Intitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommentText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductCode = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.CommentId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductCode = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CommentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductCode);
                    table.ForeignKey(
                        name: "FK_Products_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "CommentId");
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "CommentId", "CommentText", "CreatedDate", "ProductCode", "SessionId" },
                values: new object[] { 1, "Value for money", new DateTime(2023, 7, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 12345, null });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductCode", "CommentId", "ProductDescription", "ProductName", "ProductPrice", "UpdatedDate" },
                values: new object[] { 12345, null, "Beyerdynamic's flagship open studio headphone, the Beyerdynamic DT 1990 PRO is a comfortable and accurate closed studio headphone designed for hours of mixing and critical listening.", "Beyerdynamic DT 1990 PRO ", 849.5m, new DateTime(2023, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CommentId",
                table: "Products",
                column: "CommentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Comments");
        }
    }
}
