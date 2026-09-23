using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConceptionQuiz_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCategorieToQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "categorieId",
                table: "questions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_questions_categorieId",
                table: "questions",
                column: "categorieId");

            migrationBuilder.AddForeignKey(
                name: "FK_questions_categories_categorieId",
                table: "questions",
                column: "categorieId",
                principalTable: "categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_questions_categories_categorieId",
                table: "questions");

            migrationBuilder.DropIndex(
                name: "IX_questions_categorieId",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "categorieId",
                table: "questions");
        }
    }
}
