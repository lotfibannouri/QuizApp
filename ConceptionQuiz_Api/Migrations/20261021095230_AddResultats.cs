using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConceptionQuiz_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddResultats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "resultats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    quizid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    quizTitre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    categorieId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    userid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    tentative = table.Column<int>(type: "int", nullable: false),
                    startedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    submittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dureeSecondes = table.Column<int>(type: "int", nullable: false),
                    dureeAutoriseeMinutes = table.Column<int>(type: "int", nullable: false),
                    isAutoSubmitted = table.Column<bool>(type: "bit", nullable: false),
                    score = table.Column<double>(type: "float", nullable: false),
                    scoreMax = table.Column<double>(type: "float", nullable: false),
                    pourcentage = table.Column<double>(type: "float", nullable: false),
                    mention = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    isReussi = table.Column<bool>(type: "bit", nullable: false),
                    nbQuestions = table.Column<int>(type: "int", nullable: false),
                    nbCorrectes = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resultats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_resultats_categories_categorieId",
                        column: x => x.categorieId,
                        principalTable: "categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_resultats_quiz_quizid",
                        column: x => x.quizid,
                        principalTable: "quiz",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "resultatsQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    resultatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    questionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ordre = table.Column<int>(type: "int", nullable: false),
                    questionTexte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    questionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    categorieId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    score = table.Column<double>(type: "float", nullable: false),
                    scoreMax = table.Column<double>(type: "float", nullable: false),
                    isCorrecte = table.Column<bool>(type: "bit", nullable: false),
                    isRepondue = table.Column<bool>(type: "bit", nullable: false),
                    reponseDonnee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sortieObtenue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resultatsQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_resultatsQuestions_questions_questionId",
                        column: x => x.questionId,
                        principalTable: "questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_resultatsQuestions_resultats_resultatId",
                        column: x => x.resultatId,
                        principalTable: "resultats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_resultats_categorieId",
                table: "resultats",
                column: "categorieId");

            migrationBuilder.CreateIndex(
                name: "IX_resultats_quizid_submittedAt",
                table: "resultats",
                columns: new[] { "quizid", "submittedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_resultats_sessionId",
                table: "resultats",
                column: "sessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_resultats_userid_submittedAt",
                table: "resultats",
                columns: new[] { "userid", "submittedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_resultatsQuestions_questionId",
                table: "resultatsQuestions",
                column: "questionId");

            migrationBuilder.CreateIndex(
                name: "IX_resultatsQuestions_resultatId",
                table: "resultatsQuestions",
                column: "resultatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "resultatsQuestions");

            migrationBuilder.DropTable(
                name: "resultats");
        }
    }
}
