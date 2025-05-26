# WebSudoku

A web-based Sudoku puzzle manager and solver built with ASP.NET Core Razor Pages (.NET 9, C# 13). This application allows users to select, add, update, delete, and solve Sudoku puzzles through an interactive web interface.

## Features

- **Puzzle Selector:** Choose from a list of available Sudoku puzzles.
- **Add/Update/Delete:** Manage your own Sudoku puzzles and their difficulty ratings.
- **Solver:** Instantly solve any selected puzzle.
- **Reset:** Restore the puzzle to its original state.
- **Responsive UI:** Built with Razor Pages, JavaScript, and CSS for a smooth user experience.

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio 2022 or later

### Running the Application

1. **Clone the repository:**
	1. git clone <repository-url> cd WebSudoku_v0.0.7
2. **Restore dependencies:**
	1. dotnet restore
3. **Build and run:**
	1. dotnet run dev
4. Open your browser and navigate to `https://localhost:7120` (or the URL shown in the console).

## Project Structure

- `Views/Home/Index.cshtml` – Main UI for puzzle selection and interaction.
- `Controllers/SudokuController.cs` – API endpoints for puzzle management and solving.
- `Repositories/SudokuPuzzlesRepository.cs` – Handles puzzle data storage and retrieval.
- `Classes/SudokuBoard.cs`, `SudokuManager.cs` – Core logic for Sudoku board and solving.
- `wwwroot/js/site.js` – JavaScript for dynamic UI updates.
- `wwwroot/css/site.css` – Custom styles.

## Usage

- **Select a puzzle:** Use the dropdown to pick a puzzle.
- **Add a puzzle:** Click "Add...", enter the puzzle and rating, then save.
- **Update a puzzle:** Select a puzzle, click "Update...", modify, and save.
- **Delete a puzzle:** Select and click "Delete".
- **Solve:** Click "Solve" to auto-solve the current puzzle.
- **Reset:** Click "Reset" to revert to the original puzzle.

## Configuration

- Application settings can be adjusted in `appsettings.json`.
- Development-specific settings are managed in `Classes/DevConfiguration.cs`.

## Contributing

Feel free to copy and make your own! I'm not looking for a collaboration, just making this open and available for personal use.

## License

This project is licensed under the MIT License.
