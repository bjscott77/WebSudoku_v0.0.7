using Microsoft.Extensions.DependencyInjection;
using WebSudoku_v0._0._7;
using WebSudoku_v0._0._7.Classes;
using Xunit;
using System.Linq;
using System.Diagnostics;

namespace WebSudoku_v0._0._7.Tests
{
    public class SudokuManagerTesting
    {
        private readonly ISudokuManager _sudokuManager;
        private readonly ISudokuBoard _sudokuBoard;
        private string testPuzzle =
            "534000902070195308008302567859761423426853791713924856961537284287419635345286179";

        public SudokuManagerTesting()
        {
            // Build a service provider using the test project's DI setup
            var services = new ServiceCollection();

            // Fix for CS0436 and CS0117:
            // Explicitly reference the correct Program class from the main project
            var program = Program.CreateHostBuilder(new string[0]).Build();
            var serviceProvider = program.Services;

            // Resolve ISudokuManager from the DI container
            _sudokuManager = serviceProvider.GetRequiredService<ISudokuManager>();
            _sudokuBoard = serviceProvider.GetRequiredService<ISudokuBoard>(); 
        }

        [Fact]
        public void InitialOddsSetup_DoesNotThrow()
        {
            var cells = new Cells();
            cells.List.Add(new Cell(new CellLocation(1, 1, 1, 0)));
            int index = 0;
            var ex = Record.Exception(() => _sudokuManager.InitialOddsSetup(ref cells, index));
            Assert.Null(ex);
        }

        [Fact]
        public void SetNextCell_ReturnsCell()
        {
            string puzzle = "1";
            int index = 0;
            var cell = _sudokuManager.SetNextCell(puzzle, index);
            Assert.NotNull(cell);
            Assert.IsType<Cell>(cell);
        }

        [Fact]
        public void SetCellOdds_DoesNotThrow()
        {
            var cells = new Cells();
            cells.List.Add(new Cell(new CellLocation(1, 1, 1, 0)));
            int index = 0;
            var ex = Record.Exception(() => _sudokuManager.SetCellOdds(ref cells, index));
            Assert.Null(ex);
        }

        [Fact]
        public void RunSolution_ReturnsCells()
        {
            var board = new Cells();
            // Add 81 empty cells for a standard Sudoku board
            for (int i = 0; i < 81; i++)
            {
                board.List.Add(new Cell(new CellLocation((i / 9) + 1, (i % 9) + 1, ((i / 27) * 3 + (i % 9) / 3) + 1, i)));
            }
            var result = _sudokuManager.RunSolution(board);
            Assert.NotNull(result);
            Assert.IsType<Cells>(result);
        }

        [Fact]
        public void InitialOddsSetup_WithEmptyCellsList_DoesNotThrow()
        {
            var cells = new Cells();
            int index = 0;
            var ex = Record.Exception(() => _sudokuManager.InitialOddsSetup(ref cells, index));
            Assert.Null(ex);
        }

        [Fact]
        public void SetNextCell_WithInvalidValue_ReturnsCellWithValueZero()
        {
            string puzzle = "X";
            int index = 0;
            var cell = _sudokuManager.SetNextCell(puzzle, index);
            Assert.NotNull(cell);
            Assert.Equal(0, cell.Value);
            Assert.False(cell.hasValue);
        }

        [Fact]
        public void SetNextCell_WithNullValue_ReturnsCellWithValueZero()
        {
            string puzzle = null;
            int index = 0;
            var cell = _sudokuManager.SetNextCell(puzzle, index);
            Assert.NotNull(cell);
            Assert.Equal(0, cell.Value);
            Assert.False(cell.hasValue);
        }

        [Fact]
        public void SetCellOdds_WithNoPossibilities_DoesNotThrow()
        {
            var cells = new Cells();
            var cell = new Cell(new CellLocation(1, 1, 1, 0));
            cell.CellPossibilities.List.Clear();
            cells.List.Add(cell);
            int index = 0;
            var ex = Record.Exception(() => _sudokuManager.SetCellOdds(ref cells, index));
            Assert.Null(ex);
        }

        [Fact]
        public void RunSolution_WithInvalidBoard_DoesNotThrow()
        {
            var board = new Cells();
            // Add 81 cells with duplicate values in a row (invalid board)
            for (int i = 0; i < 81; i++)
            {
                var cell = new Cell(new CellLocation((i / 9) + 1, (i % 9) + 1, ((i / 27) * 3 + (i % 9) / 3) + 1, i));
                cell.Value = 1; // All cells have the same value
                cell.hasValue = true;
                board.List.Add(cell);
            }
            var ex = Record.Exception(() => _sudokuManager.RunSolution(board));
            Assert.Null(ex);
        }

        [Fact]
        public void RunSolution_WithEmptyCellsList_DoesNotThrow()
        {
            var board = new Cells();
            var ex = Record.Exception(() => _sudokuManager.RunSolution(board));
            Assert.Null(ex);
        }

        [Fact]
        public void SetCellOdds_WithIndexOutOfRange_DoesNotThrow()
        {
            var cells = new Cells();
            // No cells in the list, index is out of range
            int index = 0;
            var ex = Record.Exception(() => _sudokuManager.SetCellOdds(ref cells, index));
            Assert.Null(ex);
        }

        [Fact]
        public void ProcessValueCheck_WithSingleValueInRowColumnBlock_HighlightsCorrectCells()
        {
            // Arrange
            var cells = new Cells();
            // Create a 3x3 board (for simplicity)
            for (int i = 0; i < 9; i++)
            {
                int row = (i / 3) + 1;
                int col = (i % 3) + 1;
                int block = 1;
                var cell = new Cell(new CellLocation(row, col, block, i));
                cell.Value = 0;
                cell.hasValue = false;
                cells.List.Add(cell);
            }
            // Set a value in the center cell
            var centerCell = cells.List[4];
            centerCell.Value = 1;
            centerCell.hasValue = true;

            // Act
            var result = _sudokuManager.ProcessValueCheck(ref cells);

            // Assert
            // All cells in the same row, column, and block as the center cell should be highlighted
            var highlightedCells = cells.List.Where(c => c.isHighlighted).ToList();
            Assert.Contains(centerCell, highlightedCells);
            Assert.Equal(9, highlightedCells.Count); // center, row (2 others), column (2 others), block (already included)
        }

        [Fact]
        public void ProcessValueCheck_WithNoValues_ReturnsFalseAndNoHighlights()
        {
            // Arrange
            var cells = new Cells();
            for (int i = 0; i < 9; i++)
            {
                var cell = new Cell(new CellLocation((i / 3) + 1, (i % 3) + 1, 1, i));
                cell.Value = 0;
                cell.hasValue = false;
                cells.List.Add(cell);
            }

            // Act
            var result = _sudokuManager.ProcessValueCheck(ref cells);

            // Assert
            Assert.False(result);
            Assert.All(cells.List, c => Assert.False(c.isHighlighted));
        }
        
        [Fact]
        public void ProcessOdds_WithSinglePossibility_SetsCellValueAndReturnsTrue()
        {
            // Arrange
            var cells = new Cells();
            var cell = new Cell(new CellLocation(1, 1, 1, 0));
            cell.hasValue = false;
            cell.Value = 0;
            cell.CellPossibilities.List.Clear();
            cell.CellPossibilities.List.Add(5); // Only one possible value
            cells.List.Add(cell);

            // Act
            var result = _sudokuManager.ProcessOdds(ref cells);

            // Assert
            Assert.True(result);
            Assert.True(cells.List[0].hasValue);
            Assert.Equal(5, cells.List[0].Value);
        }

        [Fact]
        public void ProcessOdds_WithMultiplePossibilities_DoesNotSetValueAndReturnsFalse()
        {
            // Arrange
            var cells = new Cells();
            var cell = new Cell(new CellLocation(1, 1, 1, 0));
            cell.hasValue = false;
            cell.Value = 0;
            cell.CellPossibilities.List.Clear();
            cell.CellPossibilities.List.Add(2);
            cell.CellPossibilities.List.Add(3); // More than one possibility
            cells.List.Add(cell);

            // Act
            var result = _sudokuManager.ProcessOdds(ref cells);

            // Assert
            Assert.False(result);
            Assert.False(cells.List[0].hasValue);
            Assert.Equal(0, cells.List[0].Value);
        }

        [Fact]
        public void ProcessOdds_WithNoPossibilities_DoesNotThrowAndReturnsFalse()
        {
            // Arrange
            var cells = new Cells();
            var cell = new Cell(new CellLocation(1, 1, 1, 0));
            cell.hasValue = false;
            cell.Value = 0;
            cell.CellPossibilities.List.Clear(); // No possibilities
            cells.List.Add(cell);

            // Act
            var result = _sudokuManager.ProcessOdds(ref cells);

            // Assert
            Assert.False(result);
            Assert.False(cells.List[0].hasValue);
            Assert.Equal(0, cells.List[0].Value);
        }

        [Fact]
        public void ProcessOdds_WithAllCellsHavingValues_ReturnsFalse()
        {
            // Arrange
            var cells = new Cells();
            for (int i = 0; i < 3; i++)
            {
                var cell = new Cell(new CellLocation(1, i + 1, 1, i));
                cell.hasValue = true;
                cell.Value = i + 1;
                cell.CellPossibilities.List.Clear();
                cell.CellPossibilities.List.Add(i + 1);
                cells.List.Add(cell);
            }

            // Act
            var result = _sudokuManager.ProcessOdds(ref cells);

            // Assert
            Assert.False(result);
            Assert.All(cells.List, c => Assert.True(c.hasValue));
        }

        [Fact]
        public void ProcessDualOdds_WithTwoPossibilities_SetsCellValueAndReturnsTrue()
        {
            // Arrange
            var cells = new Cells();
            var cell = new Cell(new CellLocation(1, 1, 1, 0));
            cell.hasValue = false;
            cell.Value = 0;
            cell.CellPossibilities.List.Clear();
            cell.CellPossibilities.List.Add(2);
            cell.CellPossibilities.List.Add(5); // Exactly two possibilities
            cells.List.Add(cell);

            // Act
            var result = _sudokuManager.ProcessDualOdds(ref cells);

            // Assert
            Assert.True(result);
            Assert.True(cells.List[0].hasValue);
            Assert.Contains(cells.List[0].Value, new[] { 2, 5 });
        }

        [Fact]
        public void ProcessDualOdds_WithNoDualOddsCells_ReturnsFalse()
        {
            // Arrange
            var cells = new Cells();
            var cell = new Cell(new CellLocation(1, 1, 1, 0));
            cell.hasValue = false;
            cell.Value = 0;
            cell.CellPossibilities.List.Clear();
            cell.CellPossibilities.List.Add(3); // Only one possibility
            cells.List.Add(cell);

            // Act
            var result = _sudokuManager.ProcessDualOdds(ref cells);

            // Assert
            Assert.False(result);
            Assert.False(cells.List[0].hasValue);
        }

        [Fact]
        public void ProcessDualOdds_WithCellAlreadyHavingValue_DoesNotChangeValueAndReturnsFalse()
        {
            // Arrange
            var cells = new Cells();
            var cell = new Cell(new CellLocation(1, 1, 1, 0));
            cell.hasValue = true;
            cell.Value = 7;
            cell.CellPossibilities.List.Clear();
            cell.CellPossibilities.List.Add(2);
            cell.CellPossibilities.List.Add(7);
            cells.List.Add(cell);

            // Act
            var result = _sudokuManager.ProcessDualOdds(ref cells);

            // Assert
            Assert.False(result);
            Assert.True(cells.List[0].hasValue);
            Assert.Equal(7, cells.List[0].Value);
        }

        [Fact]
        public void ProcessDualOdds_WithEmptyCellsList_ReturnsFalse()
        {
            // Arrange
            var cells = new Cells();

            // Act
            var result = _sudokuManager.ProcessDualOdds(ref cells);

            // Assert
            Assert.False(result);
        }


    }
}
