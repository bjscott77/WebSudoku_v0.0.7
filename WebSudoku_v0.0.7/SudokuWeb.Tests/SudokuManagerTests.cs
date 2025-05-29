using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
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
            // Use reflection to access private method
            var method = typeof(SudokuManager).GetMethod("SetColumn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (int)method.Invoke(manager, new object[] { index });
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
            var method = typeof(SudokuManager).GetMethod("SetColumn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (int)method.Invoke(manager, new object[] { index });
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
            // Use reflection to access private method
            var method = typeof(SudokuManager).GetMethod("SetBlock", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (int)method.Invoke(manager, new object[] { index });
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
            var method = typeof(SudokuManager).GetMethod("SetBlock", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (int)method.Invoke(manager, new object[] { index });
            Assert.Equal(expectedBlock, result);
        }

        [Theory]
        [InlineData("5", 0, 1, 1, 1, 0, true)]   // Valid: first cell
        [InlineData("0", 80, 9, 9, 9, 80, false)] // Valid: last cell, empty
        public void SetNextCell_ValidIndex_SetsCellCorrectly(string value, int index, int expectedRow, int expectedCol, int expectedBlock, int expectedCellIndex, bool expectedHasValue)
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var cell = manager.SetNextCell(value, index);
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
        public void SetNextCell_InvalidIndex_ReturnsDisabledCell(string value, int index)
        {
            var devConfig = _serviceProvider.GetRequiredService<DevConfiguration>();
            var manager = new SudokuManager(devConfig);
            var cell = manager.SetNextCell(value, index);
            // Should return a disabled cell with default location
            Assert.False(cell.isEnabled);
            Assert.False(cell.hasValue);
            Assert.Equal(0, cell.Location.Row);
            Assert.Equal(0, cell.Location.Column);
            Assert.Equal(0, cell.Location.Block);
            Assert.Equal(0, cell.Location.Index);
        }

        [Fact]
        public void InitialOddsSetup_SetsInitialPossibilities_WhenCellHasNoValue()
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
            manager.InitialOddsSetup(ref cells, 0);

            // Assert
            Assert.Equal(devConfig.SudokuSettings.CellStatisticsInitial, cells.List[0].CellPossibilities.List);
        }

        [Fact]
        public void InitialOddsSetup_SetsEmptyPossibilities_WhenCellHasValue()
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
            manager.InitialOddsSetup(ref cells, 0);

            // Assert
            Assert.Equal(devConfig.SudokuSettings.CellStatisticsEmpty, cells.List[0].CellPossibilities.List);
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
            var manager = new SudokuManager(devConfig);
            int size = manager.Dimensions.RowSize;
            int blockSize = manager.Dimensions.BlockSize;

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
    }
}



