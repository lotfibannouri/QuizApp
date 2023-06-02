using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConceptionQuiz_Api.Migrations
{
    /// <inheritdoc />
    public partial class tableQuizUseradded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_propositions_questions_QuestionId",
                table: "propositions");

            migrationBuilder.RenameColumn(
                name: "QuestionId",
                table: "propositions",
                newName: "questionId");

            migrationBuilder.RenameIndex(
                name: "IX_propositions_QuestionId",
                table: "propositions",
                newName: "IX_propositions_questionId");

            migrationBuilder.CreateTable(
                name: "QuizUser",
                columns: table => new
                {
                    quizid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    userid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizUser", x => new { x.quizid, x.userid });
                    table.ForeignKey(
                        name: "FK_QuizUser_quiz_quizid",
                        column: x => x.quizid,
                        principalTable: "quiz",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_propositions_questions_questionId",
                table: "propositions",
                column: "questionId",
                principalTable: "questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_propositions_questions_questionId",
                table: "propositions");

            migrationBuilder.DropTable(
                name: "QuizUser");

            migrationBuilder.RenameColumn(
                name: "questionId",
                table: "propositions",
                newName: "QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_propositions_questionId",
                table: "propositions",
                newName: "IX_propositions_QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_propositions_questions_QuestionId",
                table: "propositions",
                column: "QuestionId",
                principalTable: "questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
