using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSudoku_v0._0._7.Migrations
{
    /// <inheritdoc />
    public partial class addedPossibilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Possibles",
                table: "SudokuPuzzles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Possibles",
                table: "SudokuPuzzles");
        }
    }
}
