using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConceptionQuiz_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCategorieTableAndLinkQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "categorie",
                table: "quiz");

            migrationBuilder.AddColumn<Guid>(
                name: "categorieId",
                table: "quiz",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    titre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_quiz_categorieId",
                table: "quiz",
                column: "categorieId");

            migrationBuilder.AddForeignKey(
                name: "FK_quiz_categories_categorieId",
                table: "quiz",
                column: "categorieId",
                principalTable: "categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_quiz_categories_categorieId",
                table: "quiz");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropIndex(
                name: "IX_quiz_categorieId",
                table: "quiz");

            migrationBuilder.DropColumn(
                name: "categorieId",
                table: "quiz");

            migrationBuilder.AddColumn<string>(
                name: "categorie",
                table: "quiz",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
