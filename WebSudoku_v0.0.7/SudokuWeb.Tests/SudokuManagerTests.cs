using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using WebSudoku_v0._0._7.Classes;
using WebSudoku_v0._0._7.Data;
using WebSudoku_v0._0._7.Repositories;
using Xunit;

namespace SudokuWeb.Tests
{
    public class SudokuManagerTests
    {
        private readonly ServiceProvider _serviceProvider;

        public SudokuManagerTests()
        {
            // Load configuration from appsettings.json or environment
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Use the same connection string as your main app
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            var services = new ServiceCollection();

            // Register ApplicationDbContext with external SQL provider
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Register configuration section and DevConfiguration
            services.AddSingleton<IConfigurationSection>(configuration.GetSection("DEV"));
            var devConfig = new DevConfiguration(configuration.GetSection("DEV"));
            services.AddSingleton(devConfig);

            // Register other services as in Program.cs
            services.AddScoped<ISudokuBoard, SudokuBoard>();
            services.AddScoped<ISudokuRepository, SudokuPuzzlesRepository>();
            services.AddScoped<ISudokuManager, SudokuManager>();

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void CanResolveSudokuManager()
        {
            var manager = _serviceProvider.GetRequiredService<ISudokuManager>();
            Assert.NotNull(manager);
        }

        [Fact]
        public void CanResolveSudokuBoard()
        {
            var sudokuBoard = _serviceProvider.GetRequiredService<ISudokuBoard>();
            Assert.NotNull(sudokuBoard);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(8, 1)]
        [InlineData(9, 2)]
        [InlineData(80, 9)]
        public void SetRow_ReturnsExpectedRow(int index, int expectedRow)
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var result = manager.SetRow(index);
            Assert.Equal(expectedRow, result);
        }

        [Theory]
        [InlineData(-1, -1)]    // Negative index, should return 0 or throw
        [InlineData(0, 1)]     // First cell
        [InlineData(8, 1)]     // Last cell in first row
        [InlineData(9, 2)]     // First cell in second row
        [InlineData(71, 8)]    // Last cell in 8th row
        [InlineData(80, 9)]    // Last cell in last row
        [InlineData(81, -1)]   // Out of bounds (should be handled)
        public void SetRow_EdgeCases(int index, int expectedRow)
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var result = manager.SetRow(index);
            Assert.Equal(expectedRow, result);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(8, 9)]
        [InlineData(9, 1)]
        [InlineData(80, 9)]
        public void SetColumn_ReturnsExpectedColumn(int index, int expectedColumn)
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var result = manager.SetColumn(index);
            Assert.Equal(expectedColumn, result);
        }

        [Theory]
        [InlineData(-1, -1)]    // Negative index, should return fail indication
        [InlineData(0, 1)]     // First cell
        [InlineData(8, 9)]     // Last cell in first row
        [InlineData(9, 1)]     // First cell in second row
        [InlineData(17, 9)]    // Last cell in second row
        [InlineData(80, 9)]    // Last cell in last row
        [InlineData(81, -1)]    // Out of bounds (should wrap to first column)
        public void SetColumn_EdgeCases(int index, int expectedColumn)
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var result = manager.SetColumn(index);
            Assert.Equal(expectedColumn, result);
        }

        [Theory]
        [InlineData(0, 1)]   // Top-left cell
        [InlineData(4, 2)]   // Center of top row
        [InlineData(10, 1)]  // Second row, first block
        [InlineData(80, 9)]  // Bottom-right cell
        public void SetBlock_ReturnsExpectedBlock(int index, int expectedBlock)
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var result = manager.SetBlock(index);
            Assert.Equal(expectedBlock, result);
        }

        [Theory]
        [InlineData(-1, -1)]    // Negative index, should return 0 or throw
        [InlineData(0, 1)]     // Top-left cell, block 1
        [InlineData(2, 1)]     // First row, block 1
        [InlineData(3, 2)]     // First row, block 2
        [InlineData(8, 3)]     // First row, block 3
        [InlineData(9, 1)]     // Second row, block 1
        [InlineData(27, 4)]    // Fourth row, block 4
        [InlineData(80, 9)]    // Last cell, block 9
        [InlineData(81, -1)]   // Out of bounds (should be handled)
        public void SetBlock_EdgeCases(int index, int expectedBlock)
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var result = manager.SetBlock(index);
            Assert.Equal(expectedBlock, result);
        }

        [Theory]
        [InlineData("5", 0, 1, 1, 1, 0, true)]   // Valid: first cell
        [InlineData("0", 80, 9, 9, 9, 80, false)] // Valid: last cell, empty
        public void CreateNextCell_ValidIndex_SetsCellCorrectly(string value, int index, int expectedRow, int expectedCol, int expectedBlock, int expectedCellIndex, bool expectedHasValue)
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var cell = manager.createNextCell(value, index);
            Assert.Equal(expectedRow, cell.Location.Row);
            Assert.Equal(expectedCol, cell.Location.Column);
            Assert.Equal(expectedBlock, cell.Location.Block);
            Assert.Equal(expectedCellIndex, cell.Location.Index);
            Assert.Equal(expectedHasValue, cell.hasValue);
        }

        [Theory]
        [InlineData("1", -1)]
        [InlineData("2", 81)]
        [InlineData("3", 100)]
        public void CreateNextCell_InvalidIndex_ReturnsDisabledCell(string value, int index)
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var cell = manager.createNextCell(value, index);
            // Should return a disabled cell with default location
            Assert.False(cell.isEnabled);
            Assert.False(cell.hasValue);
            Assert.Equal(0, cell.Location.Row);
            Assert.Equal(0, cell.Location.Column);
            Assert.Equal(0, cell.Location.Block);
            Assert.Equal(0, cell.Location.Index);
        }

        [Fact]
        public void SetupProbabilities_SetsInitialPossibilities_WhenCellHasNoValue()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);

            // Arrange: create a cell with hasValue = false
            var cell = new Cell(new CellLocation(1, 1, 1, 0))
            {
                hasValue = false,
                CellPossibilities = new CellPossibilities { List = new List<int>() }
            };
            var cells = new Cells { List = new List<Cell> { cell } };

            // Act
            manager.SetupProbabilities(ref cells, 0);

            // Assert
            Assert.Equal(devConfig.SudokuSettings.CellStatisticsInitial, cells.List[0].CellPossibilities.List);
        }

        [Fact]
        public void SetupProbabilities_SetsEmptyPossibilities_WhenCellHasValue()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);

            // Arrange: create a cell with hasValue = true
            var cell = new Cell(new CellLocation(1, 1, 1, 0))
            {
                hasValue = true,
                CellPossibilities = new CellPossibilities { List = new List<int>() }
            };
            var cells = new Cells { List = new List<Cell> { cell } };

            // Act
            manager.SetupProbabilities(ref cells, 0);

            // Assert
            Assert.Equal(devConfig.SudokuSettings.CellStatisticsEmpty, cells.List[0].CellPossibilities.List);
        }

        [Fact]
        public void ClearCellOdds_ZerosPossibilities_WhenCellHasValue()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);

            // Arrange: create a board with two cells
            var cell1 = new Cell(new CellLocation(1, 1, 1, 0))
            {
                Value = 1,
                hasValue = true,
                isEnabled = true,
                CellPossibilities = new CellPossibilities { List = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 } }
            };
            // Act
            var result = (Cell)typeof(SudokuManager)
                .GetMethod("ClearCellOdds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { cell1 });

            // Assert
            Assert.Equal(result.CellPossibilities.List.Select(c => c == 0).Count(), 9);
            Assert.Equal(result.CellPossibilities.List.Count(), 9);
        }

        [Fact]
        public void ClearCellOdds_IgnoresPossibilities_WhenCellHasNoValue()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);

            // Arrange: create a board with two cells
            var cell1 = new Cell(new CellLocation(1, 1, 1, 0))
            {
                Value = 0,
                hasValue = false,
                isEnabled = false,
                CellPossibilities = new CellPossibilities { List = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 } }
            };
            // Act
            var result = (Cell)typeof(SudokuManager)
                .GetMethod("ClearCellOdds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { cell1 });

            // Assert
            Assert.Equal(result.CellPossibilities.List.Where(c => c == 0).Count(), 0);
            Assert.Equal(result.CellPossibilities.List.Count(), 9);
        }

        [Fact]
        public void DeepCopyCells_NotEqualToCells_WhenValueUpdated()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var board = new SudokuBoard(devConfig);
            var manager = new SudokuManager(devConfig);

            // Arrange
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();
            var cells = board.Cells;
            // Act
            var result = (List<Cell>)typeof(SudokuManager)
                .GetMethod("DeepCopyCells", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(board.SudokuManager, new object[] { cells });

            result[1].Value = 2;
            result[1].hasValue = true;
            board.SudokuManager.SetCellProbabilities(ref cells, result[1].Location.Index);

            // Assert
            Assert.NotEqual(result, board.Cells.List);
        }

        [Fact]
        public void CompareBoardCells_ReturnsTrue_WhenCurrentEqualsPrevious()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var board = new SudokuBoard(devConfig);
            var manager = new SudokuManager(devConfig);

            // Arrange
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();
            var previous = board.Cells.List;

            // Act
            var current = (List<Cell>)typeof(SudokuManager)
                .GetMethod("DeepCopyCells", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(board.SudokuManager, new object[] { board.Cells });

            var result = (bool)typeof(SudokuManager)
                .GetMethod("CompareBoardCells", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(board.SudokuManager, new object[] { previous, current });

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CompareBoardCells_ReturnsFalse_WhenNotCurrentEqualsPrevious()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var board = new SudokuBoard(devConfig);
            var manager = new SudokuManager(devConfig);

            // Arrange
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();
            var previous = board.Cells.List;

            // Act
            var current = (List<Cell>)typeof(SudokuManager)
                .GetMethod("DeepCopyCells", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(board.SudokuManager, new object[] { board.Cells });

            current[1].Value = 2;
            current[1].hasValue = true;

            var result = (bool)typeof(SudokuManager)
                .GetMethod("CompareBoardCells", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(board.SudokuManager, new object[] { previous, current });

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void DifferenceBoardCells_ReturnsDifferenceString_WhenNotCurrentEqualsPrevious()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var board = new SudokuBoard(devConfig);
            var manager = new SudokuManager(devConfig);

            // Arrange
            var testPuzzle = "X2XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();
            var previous = board.Cells.List;

            // Act
            var current = (List<Cell>)typeof(SudokuManager)
                .GetMethod("DeepCopyCells", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(board.SudokuManager, new object[] { board.Cells });

            current[1].Value = 2;
            current[1].hasValue = true;

            var result = (string)typeof(SudokuManager)
                .GetMethod("DifferenceBoardCells", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(board.SudokuManager, new object[] { current, previous });

            // Assert
            Assert.Equal(result, testPuzzle);
        }

        [Fact]
        public void HasCorruptedOdds_ReturnsTrue_WhenCorruptOddsFound()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var board = new SudokuBoard(devConfig);
            var manager = new SudokuManager(devConfig);

            // Arrange
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();

            // Act
            board.Cells.List[1].CellPossibilities.List = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            var result = (bool)typeof(SudokuManager)
                .GetMethod("HasCorruptedOdds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells });

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void HasCorruptedOdds_ReturnsFalse_WhenNoCorruptOddsFound()  
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var board = new SudokuBoard(devConfig);
            var manager = new SudokuManager(devConfig);

            // Arrange
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();

            // Act
            var result = (bool)typeof(SudokuManager)
                .GetMethod("HasCorruptedOdds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells });

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void BackupDualOdds_PushesBackupToStack_AndSetsHasBackup()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);

            // Arrange: create a board with two cells
            var cell1 = new Cell(new CellLocation(1, 1, 1, 0))
            {
                Value = 1,
                hasValue = true,
                isEnabled = true,
                CellPossibilities = new CellPossibilities { List = new List<int> { 1, 2, 3 } }
            };
            var cell2 = new Cell(new CellLocation(1, 2, 1, 1))
            {
                Value = 0,
                hasValue = false,
                isEnabled = true,
                CellPossibilities = new CellPossibilities { List = new List<int> { 1, 2 } }
            };
            var cells = new Cells { List = new List<Cell> { cell1, cell2 } };

            // Act
            var result = typeof(SudokuManager)
                .GetMethod("BackupDualOdds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { cells, cell2 });

            // Assert
            Assert.True((bool)result);
            Assert.True(cells.List[1].hasBackup);
        }

        [Fact]
        public void BackupDualOdds_ReturnsFalse_OnNullCells()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);

            // Act
            var result = typeof(SudokuManager)
                .GetMethod("BackupDualOdds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { null, null });

            // Assert
            Assert.False((bool)result);
        }

        [Fact]
        public void BackupDualOdds_ReturnsFalse_OnInvalidDualOddsCellIndex()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);

            // Arrange: cell with index not in cells.List
            var cell1 = new Cell(new CellLocation(1, 1, 1, 0))
            {
                Value = 1,
                hasValue = true,
                isEnabled = true,
                CellPossibilities = new CellPossibilities { List = new List<int> { 1, 2, 3 } }
            };
            var cells = new Cells { List = new List<Cell> { cell1 } };
            var dualOddsCell = new Cell(new CellLocation(1, 2, 1, 5))
            {
                Value = 0,
                hasValue = false,
                isEnabled = true,
                CellPossibilities = new CellPossibilities { List = new List<int> { 1, 2 } }
            };

            // Act
            var result = typeof(SudokuManager)
                .GetMethod("BackupDualOdds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { cells, dualOddsCell });

            // Assert
            Assert.False((bool)result);
        }

        [Fact]
        public void IsBoardValid_ReturnsFalse_OnInvalid()   
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var appConfig = _serviceProvider.GetRequiredService<IConfigurationSection>();

            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig) { Dimensions = manager.Dimensions };

            // Arrange: board with invalid puzzle
            var invalidPuzzle = "108077090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(invalidPuzzle);
            board.InitializeProbabilities();


            // Act
            var result = typeof(SudokuManager)
                .GetMethod("IsBoardValid", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, true });

            // Assert
            Assert.False((bool)result);
        }

        [Fact]
        public void IsBoardValid_ReturnsTrue_OnValid()  
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var appConfig = _serviceProvider.GetRequiredService<IConfigurationSection>();

            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig) { Dimensions = manager.Dimensions };

            // Arrange: board with valid puzzle
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();


            // Act
            var result = typeof(SudokuManager)
                .GetMethod("IsBoardValid", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, false });

            // Assert
            Assert.True((bool)result);
        }

        [Theory]        
        [InlineData(4, 5, 5)] 
        [InlineData(6, 3, 3)] 
        [InlineData(10, 4, 4)] 
        [InlineData(79, 5, 5)] 
        public void PlaceCellValue_UpdatesBoardWithValue_OnValidIndexAndValue(int index, int value, int expected)
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig);

            // Arrange
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();

            // Act
            var result = (bool)typeof(SudokuManager)
                .GetMethod("PlaceCellValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, index, value });

            // Assert
            Assert.True((bool)result);
            Assert.Equal(board.Cells.List[index].Value, expected);
        }

        [Theory]
        [InlineData(-1, 6, 0)]
        [InlineData(81, 5, 0)]
        public void PlaceCellValue_DoesNotUpdateBoard_OnInvalidIndex(int index, int value, int expected)
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig);

            // Arrange
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();

            // Act
            var result = (bool)typeof(SudokuManager)
                .GetMethod("PlaceCellValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, index, value });

            // Assert
            Assert.False((bool)result);
        }

        [Theory]
        [InlineData(22, 8, 0)]
        [InlineData(38, 2, 0)]
        public void PlaceCellValue_DoesNotUpdateBoard_OnInvalidValue(int index, int value, int expected)
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig);

            // Arrange
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();

            // Act
            var result = (bool)typeof(SudokuManager)
                .GetMethod("PlaceCellValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, index, value });

            // Assert
            Assert.False((bool)result);
            Assert.Equal(board.Cells.List[index].Value, expected);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(6, 4)]
        [InlineData(8, 7)]
        public void GetBlockRow_ReturnsListOfBlocks_WhenValidBlock(int block, int expected)
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig);

            // Arrange
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();

            // Act
            var result = (List<List<Cell>>)typeof(SudokuManager)
                .GetMethod("GetBlockRow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, block });

            // Assert
            Assert.Equal(result[0][0].Location.Block, expected);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(-1)]
        public void GetBlockRow_ReturnsEmptyList_WheninvalidBlock(int block)
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig);

            // Arrange
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();

            // Act
            var result = (List<List<Cell>>)typeof(SudokuManager)
                .GetMethod("GetBlockRow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, block });

            // Assert
            Assert.True(!result.Any());
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(6, 3)]
        [InlineData(8, 2)]
        public void GetBlockColumn_ReturnsListOfBlocks_WhenValidBlock(int block, int expected)
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig);

            // Arrange
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();

            // Act
            var result = (List<List<Cell>>)typeof(SudokuManager)
                .GetMethod("GetBlockColumn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, block });

            // Assert
            Assert.Equal(result[0][0].Location.Block, expected);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(-1)]
        public void GetBlockColumn_ReturnsEmptyList_WheninvalidBlock(int block)
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig);

            // Arrange
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();

            // Act
            var result = (List<List<Cell>>)typeof(SudokuManager)
                .GetMethod("GetBlockColumn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, block });

            // Assert
            Assert.True(!result.Any());
        }

        [Fact]
        public void FindInitialRowPattern_ReturnsTrue_WhenValidRowFound()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig);

            // Arrange
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();
            var initialRow = new List<Cell>();
            // Act
            var blockRow = (List<List<Cell>>)typeof(SudokuManager)
                .GetMethod("GetBlockRow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, 5 });

            var result = (bool)typeof(SudokuManager)
                .GetMethod("FindInitialRowPattern", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { initialRow, blockRow[1] });

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void FindInitialRowPattern_ReturnsFalse_WhenNoneFound()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig);

            // Arrange
            var validPuzzle = "108007090000098000060000700000086000370905082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();
            var initialRow = new List<Cell>();
            // Act
            var blockRow = (List<List<Cell>>)typeof(SudokuManager)
                .GetMethod("GetBlockRow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, 5 });

            var result = (bool)typeof(SudokuManager)
                .GetMethod("FindInitialRowPattern", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { initialRow, blockRow[1] });

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void FindInitialColumnPattern_ReturnsTrue_WhenValidRowFound()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig);

            // Arrange
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();
            var initialColumn = new List<Cell>();
            // Act
            var blockColumn = (List<List<Cell>>)typeof(SudokuManager)
                .GetMethod("GetBlockColumn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, 5 });

            var result = (bool)typeof(SudokuManager)
                .GetMethod("FindInitialColumnPattern", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { initialColumn, blockColumn[1] });

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void FindInitialColumnPattern_ReturnsFalse_WhenNoneFound()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig);

            // Arrange
            var validPuzzle = "108007090000098000060000700000086000370905082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();
            var initialColumn = new List<Cell>();
            // Act
            var blockColumn = (List<List<Cell>>)typeof(SudokuManager)
                .GetMethod("GetBlockColumn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, 5 });

            var result = (bool)typeof(SudokuManager)
                .GetMethod("FindInitialColumnPattern", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { initialColumn, blockColumn[1] });

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void FindFilledRow_ReturnsListOfCell_WhenFound()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var appConfig = _serviceProvider.GetRequiredService<IConfigurationSection>();

            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig) { Dimensions = manager.Dimensions };
            var cells = board.Cells;

            // Arrange
            var validPuzzle = "000547096743090021005002740000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();

            List<List<List<Cell>>> blockRows = new List<List<List<Cell>>>();
            var finalRow = new List<Cell>();
            var blockRow = (List<List<Cell>>)typeof(SudokuManager)
                .GetMethod("GetBlockRow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, 1 });
            blockRows.Add(blockRow);

            var filledRow = (List<Cell>)typeof(SudokuManager)
                .GetMethod("FindFilledRow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { blockRows });

            // Assert
            Assert.True(filledRow.Any());
            Assert.True(filledRow.Count() == 3);
            Assert.Equal(filledRow[0].Value, 7);
            Assert.Equal(filledRow[1].Value, 4);
            Assert.Equal(filledRow[2].Value, 3);


        }

        [Fact]
        public void FindOpposing_ReturnsListOfCell_WhenFound()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var appConfig = _serviceProvider.GetRequiredService<IConfigurationSection>();

            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig) { Dimensions = manager.Dimensions };
            var cells = board.Cells;

            // Arrange
            var validPuzzle = "000547096743090021005002740000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();

            List<List<List<Cell>>> blockRows = new List<List<List<Cell>>>();
            var finalRow = new List<Cell>();
            var blockRow = (List<List<Cell>>)typeof(SudokuManager)
                .GetMethod("GetBlockRow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, 1 });
            blockRows.Add(blockRow);

            var filledRow = (List<Cell>)typeof(SudokuManager)
                .GetMethod("FindFilledRow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { blockRows });

            var opposingRow = (List<Cell>)typeof(SudokuManager)
                .GetMethod("FindOpposing", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { blockRows, filledRow[0].Location.Block, filledRow[0].Location.Row, false });

            // Assert
            Assert.True(opposingRow.Any());
            Assert.True(opposingRow.Count() == 3);
            Assert.Equal(opposingRow[0].Value, 5);
            Assert.Equal(opposingRow[1].Value, 4);
            Assert.Equal(opposingRow[2].Value, 7);
        }

        [Fact]
        public void FindPatternValue_ReturnsValue_WhenFound()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var appConfig = _serviceProvider.GetRequiredService<IConfigurationSection>();

            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig) { Dimensions = manager.Dimensions };
            var cells = board.Cells;

            // Arrange
            var validPuzzle = "000547096743090021005002740000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();

            List<List<List<Cell>>> blockRows = new List<List<List<Cell>>>();
            var finalRow = new List<Cell>();
            var blockRow = (List<List<Cell>>)typeof(SudokuManager)
                .GetMethod("GetBlockRow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, 1 });
            blockRows.Add(blockRow);

            var filledRow = (List<Cell>)typeof(SudokuManager)
                .GetMethod("FindFilledRow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { blockRows });

            var opposingRow = (List<Cell>)typeof(SudokuManager)
                .GetMethod("FindOpposing", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { blockRows, filledRow[0].Location.Block, filledRow[0].Location.Row, false });

            var result = (int)typeof(SudokuManager)
                .GetMethod("FindPatternValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { filledRow, opposingRow });

            // Assert
            Assert.NotNull(result);
            Assert.True(result > 0);
            Assert.Equal(result, 3);
        }

        [Fact]
        public void FindFilledColumn_ReturnsAListOfColumns_OnValidBlockColumn() 
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var appConfig = _serviceProvider.GetRequiredService<IConfigurationSection>();

            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig) { Dimensions = manager.Dimensions };
            var cells = board.Cells;

            // Arrange
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();

            List<List<List<Cell>>> blockColumns = new List<List<List<Cell>>>();
            var blockColumn = (List<List<Cell>>)typeof(SudokuManager)
                .GetMethod("GetBlockColumn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, 5 });
            blockColumns.Add(blockColumn);

            // Act
            var result = (List<Cell>)typeof(SudokuManager)
                .GetMethod("FindFilledColumn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { blockColumns });

            // Assert
            Assert.IsType<List<Cell>>(result);
            Assert.NotNull(result);
            Assert.Equal(result?.Count(), 3);
            Assert.Equal(result?[0].Value, 8);
            Assert.Equal(result?[1].Value, 1);
            Assert.Equal(result?[2].Value, 7);
        }

        [Fact]
        public void FindFilledColumn_ReturnsNull_OnNullBlockColumn() 
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var appConfig = _serviceProvider.GetRequiredService<IConfigurationSection>();

            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig) { Dimensions = manager.Dimensions };
            var cells = board.Cells;

            // Arrange: board with valid puzzle
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();

            // Arrange: 1.Get list of block columns
            List<List<List<Cell>>> blockColumns = null;

            // Act
            var result = typeof(SudokuManager)
                .GetMethod("FindFilledColumn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { blockColumns });

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(2)]
        public void ProcessHighlights_ReturnsTrue_WhenFoundInBlock(int value)
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var appConfig = _serviceProvider.GetRequiredService<IConfigurationSection>();

            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig) { Dimensions = manager.Dimensions };
            var cells = board.Cells;

            // Arrange: board with valid puzzle
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();
            board.Cells.List[3].isHighlighted = true;
            board.Cells.List[4].isHighlighted = true;
            board.Cells.List[5].isHighlighted = true;
            board.Cells.List[12].isHighlighted = true;
            board.Cells.List[13].isHighlighted = true;
            board.Cells.List[14].isHighlighted = true;
            board.Cells.List[21].isHighlighted = true;
            board.Cells.List[22].isHighlighted = true;

            // Arrange: 1.Get list of block columns
            List<List<List<Cell>>> blockColumns = null;

            // Act
            var result = (bool)typeof(SudokuManager)
                .GetMethod("ProcessHighlights", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, value });

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(2)]
        public void ProcessHighlights_ReturnsFalse_WhenNotFoundInBlock(int value)
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var appConfig = _serviceProvider.GetRequiredService<IConfigurationSection>();

            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig) { Dimensions = manager.Dimensions };
            var cells = board.Cells;

            // Arrange: board with valid puzzle
            var validPuzzle = "108007090000098000060000700000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();
            board.Cells.List[3].isHighlighted = true;
            board.Cells.List[4].isHighlighted = true;
            board.Cells.List[5].isHighlighted = true;
            board.Cells.List[13].isHighlighted = true;
            board.Cells.List[14].isHighlighted = true;
            board.Cells.List[21].isHighlighted = true;
            board.Cells.List[22].isHighlighted = true;

            // Arrange: 1.Get list of block columns
            List<List<List<Cell>>> blockColumns = null;

            // Act
            var result = (bool)typeof(SudokuManager)
                .GetMethod("ProcessHighlights", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, value });

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void FindSingleEmptyRow_ReturnsTrue_WhenFound()
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var appConfig = _serviceProvider.GetRequiredService<IConfigurationSection>();

            var manager = new SudokuManager(devConfig);
            var board = new SudokuBoard(devConfig) { Dimensions = manager.Dimensions };
            var cells = board.Cells;

            // Arrange
            var validPuzzle = "000547096743090021005002740000086000370915082000370000009000060000420000030700104";
            board.createSudokuBoard(validPuzzle);
            board.InitializeProbabilities();

            List<List<List<Cell>>> blockRows = new List<List<List<Cell>>>();
            var finalRow = new List<Cell>();
            var blockRow = (List<List<Cell>>)typeof(SudokuManager)
                .GetMethod("GetBlockRow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { board.Cells, 1 });
            blockRows.Add(blockRow);

            var filledRow = (List<Cell>)typeof(SudokuManager)
                .GetMethod("FindFilledRow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { blockRows });

            var opposingRow = (List<Cell>)typeof(SudokuManager)
                .GetMethod("FindOpposing", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { blockRows, filledRow[0].Location.Block, filledRow[0].Location.Row, false });

            var value = (int)typeof(SudokuManager)
                .GetMethod("FindPatternValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { filledRow, opposingRow });

            // Act
            var result = (bool)typeof(SudokuManager)
                .GetMethod("FindSingleEmptyRow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(manager, new object[] { finalRow, board.Cells, blockRows, filledRow, opposingRow, value });

            // Assert
            Assert.True(result);
        }
    }
}