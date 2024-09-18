using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TypicalTechTools.Migrations
{
    /// <inheritdoc />
    public partial class AddingUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Comments_CommentId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CommentId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Products");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "AdminUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHarsh = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommentProduct",
                columns: table => new
                {
                    CommentsCommentId = table.Column<int>(type: "int", nullable: false),
                    ProductsProductCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentProduct", x => new { x.CommentsCommentId, x.ProductsProductCode });
                    table.ForeignKey(
                        name: "FK_CommentProduct_Comments_CommentsCommentId",
                        column: x => x.CommentsCommentId,
                        principalTable: "Comments",
                        principalColumn: "CommentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentProduct_Products_ProductsProductCode",
                        column: x => x.ProductsProductCode,
                        principalTable: "Products",
                        principalColumn: "ProductCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AdminUsers",
                columns: new[] { "Id", "PasswordHarsh", "UserName" },
                values: new object[] { 1, "$2a$11$C64UEyc/o1oRERsg8FwXxOmrkD5jnnfnDFg5ayNcupeJScG6G4xTK", "Admin" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductCode",
                keyValue: 12345,
                columns: new[] { "ProductDescription", "ProductName", "ProductPrice" },
                values: new object[] { "bluetooth headphones with fair battery life and a 1 month warranty", "Generic Headphones\r\n", 84.99m });

            migrationBuilder.CreateIndex(
                name: "IX_CommentProduct_ProductsProductCode",
                table: "CommentProduct",
                column: "ProductsProductCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminUsers");

            migrationBuilder.DropTable(
                name: "CommentProduct");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "Products",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductCode",
                keyValue: 12345,
                columns: new[] { "CommentId", "ProductDescription", "ProductName", "ProductPrice" },
                values: new object[] { null, "Beyerdynamic's flagship open studio headphone, the Beyerdynamic DT 1990 PRO is a comfortable and accurate closed studio headphone designed for hours of mixing and critical listening.", "Beyerdynamic DT 1990 PRO ", 849.5m });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CommentId",
                table: "Products",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Comments_CommentId",
                table: "Products",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "CommentId");
        }
    }
}
