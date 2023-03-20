using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace authentification_Api.Migrations
{
    /// <inheritdoc />
    public partial class migrationdb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "adresse",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "dateCreation",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "nom",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "prenom",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "adresse",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "dateCreation",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "nom",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "prenom",
                table: "AspNetUsers");
        }
    }
}
