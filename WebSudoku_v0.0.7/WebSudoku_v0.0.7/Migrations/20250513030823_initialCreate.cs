using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSudoku_v0._0._7.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SudokuPuzzles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 36, nullable: false),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    BoardValues = table.Column<string>(type: "nvarchar(81)", maxLength: 81, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SudokuPuzzle", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SudokuPuzzles");
        }
    }
}
