using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConceptionQuiz_Api.Migrations
{
    /// <inheritdoc />
    public partial class DropNiveauAddCategorie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "niv_deficulte",
                table: "quiz");

            migrationBuilder.AddColumn<string>(
                name: "categorie",
                table: "quiz",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "categorie",
                table: "quiz");

            migrationBuilder.AddColumn<int>(
                name: "niv_deficulte",
                table: "quiz",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
