using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace WebSudoku_v0._0._7.Classes
{
    public class SudokuManager : ISudokuManager
    {
        #region Declarations
        private Stack<(List<Cell> Cells, Cell Cell)> DualOddsBackups = new Stack<(List<Cell>, Cell)>();
        
        private List<(int Attempt, int Index, int Value, int AlternateValue)> DualOddsRecord = new List<(int, int, int, int)>();

        public SudokuDimensions Dimensions { get; set; } = null;
        public DevConfiguration DevConfig { get; set; } 
        public SudokuManager(DevConfiguration devConfig)
        {
            DevConfig = devConfig;
            Dimensions = new SudokuDimensions(DevConfig.SudokuSettings.BoardDimensions.FirstOrDefault(), devConfig.SudokuSettings.BoardDimensions.LastOrDefault());
        }
        #endregion

        #region Initialization And Updates
        public void SetupProbabilities(ref Cells cells, int index)
        {
                try
                {
                    Cell cell = cells.List[index];
                    cells.List[index].CellPossibilities.List.Clear();
                    if (!cell.hasValue)
                    {
                        cells.List[index].CellPossibilities.List.AddRange(DevConfig.SudokuSettings.CellStatisticsInitial);
                    }
                    else
                    {
                        cells.List[index].CellPossibilities.List.AddRange(DevConfig.SudokuSettings.CellStatisticsEmpty);
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Console.WriteLine($"Error in SetUpProbabilities: {ex.Message}");

                    Cell cell = new Cell(new CellLocation(0, 0, 0, 0));
                    cell.isEnabled = false;
                    cell.hasValue = false;
                }
        }

        public Cell createNextCell(string cellValue,  int index)
        {
            Cell cell;
            try
            {
                CellLocation cellLocation = 
                    new CellLocation(
                        SetRow(index), 
                        SetColumn(index), 
                        SetBlock(index), 
                        index);
                if (cellLocation.Row == -1 || cellLocation.Column == -1 || cellLocation.Block == -1)
                    throw new ArgumentOutOfRangeException("index", $"Index must be between {0}:{Dimensions.Size - 1} inclusivley.  Expected: 0-80, Received: {index}");

                cell = new Cell(cellLocation);
                cell.DisplayValue = DevConfig.SudokuSettings.GamePlaySettings.SolveSettings.CellDisplayValueType == "SPACE"
                    ? cellValue == "0"
                        ? " " : cellValue
                    : cellValue;
                cell.Value = int.TryParse(cellValue, out int value) ? value : 0;
                cell.hasValue = !string.IsNullOrEmpty(cellValue) && cell.Value != 0;
                cell.isHighlighted = false;
                cell.isEnabled = true;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Error in SetNextCell: {ex.Message}");
                cell = new Cell(new CellLocation(0, 0, 0, 0));
                cell.isEnabled = false;
                cell.hasValue = false;
            }
            return cell;
        }

        public void SetCellProbabilities(ref Cells cells, int index)
        {
            Cell cell = cells.List[index];
            if (!cell.hasValue)
            {
                var rowCells = cells.List.Where(c => c.Location.Row == cell.Location.Row).ToList();
                foreach (var rowCell in rowCells)
                {
                    if (rowCell.hasValue)
                    {
                        var indexOf = cells.List[index].CellPossibilities.List.IndexOf(rowCell.Value);
                        if (indexOf == -1)
                        {
                            continue;
                        }
                        else
                        {
                            cells.List[index].CellPossibilities.List[indexOf] = 0;
                        }
                    }                             
                }

                var colCells = cells.List.Where(c => c.Location.Column == cell.Location.Column).ToList();
                foreach (var colCell in colCells)
                {
                    if (colCell.hasValue)
                    {
                        var indexOf = cells.List[index].CellPossibilities.List.IndexOf(colCell.Value);
                        if (indexOf == -1)
                        {
                            continue;
                        }
                        else
                        {
                            cells.List[index].CellPossibilities.List[indexOf] = 0;
                        }
                    }
                }
                var blockCells = cells.List.Where(c => c.Location.Block == cell.Location.Block).ToList();
                foreach (var blockCell in blockCells)
                {
                    if (blockCell.hasValue)
                    {
                        var indexOf = cells.List[index].CellPossibilities.List.IndexOf(blockCell.Value);
                        if (indexOf == -1)
                        {
                            continue;
                        }
                        else
                        {
                            cells.List[index].CellPossibilities.List[indexOf] = 0;
                        }
                    }
                }
            }
        }

        public int SetBlock(int index)
        {
            if (index < 0 || index > 80)
                return -1;
            // Rows, columns, and blocks are 1-based
            int row = SetRow(index);    // 1-based row
            int col = SetColumn(index); // 1-based column
            int blockRow = (row - 1) / 3;
            int blockCol = (col - 1) / 3;
            return (blockRow * 3) + blockCol + 1;
        }

        public int SetColumn(int index)
        {
            if (index < 0 || index > 80)
                return -1;

            return (index % Dimensions.ColumnSize) + 1;
        }

        public int SetRow(int index)
        {
            if (index < 0 || index > 80)
                return -1;

            return (index / Dimensions.RowSize) + 1;
        }
        #endregion

        #region Solution Helpers
        private Cell ClearCellOdds(Cell cell)
        {
            if (!cell.hasValue && cell.Value == 0)
                return cell;

            for (int i = 0; i < cell.CellPossibilities.List.Count; i++)
            {
                cell.CellPossibilities.List[i] = 0;
            }
            return cell;
        }

        private bool IsBackedUp(Cell cell)
        {
            foreach (var record in DualOddsBackups)
            {
                if (record.Cell == cell)
                    return true;
            }

            return false;
        }

        private List<Cell> DeepCopyCells(Cells cells)
        {
            var cellsCopy = new List<Cell>();
            foreach (var cell in cells.List)
            {
                var cellCopy = new Cell(
                    new CellLocation(
                        cell.Location.Row,
                        cell.Location.Column,
                        cell.Location.Block,
                        cell.Location.Index
                    )
                )
                {
                    DisplayValue = cell.DisplayValue,
                    Value = cell.Value,
                    isEnabled = cell.isEnabled,
                    hasValue = cell.hasValue,
                    isHighlighted = false,
                    CellPossibilities = new CellPossibilities
                    {
                        List = new List<int>(cell.CellPossibilities.List)
                    }
                };
                cellsCopy.Add(cellCopy);
            }
            return cellsCopy;
        }

        private bool CompareBoardCells(List<Cell> cells, List<Cell> cellsCopy)
        {
            var cellVals = string.Join("", cells.Select(c => c.Value));
            var copyVals = string.Join("", cellsCopy.Select(c => c.Value));

            return cellVals == copyVals;
        }

        private string DifferenceBoardCells(List<Cell> cells, List<Cell> cellsCopy)
        {
            var difference = string.Empty;
            cells.ForEach(cell => 
                difference += cell.Value == cellsCopy[cell.Location.Index].Value
                ? "X" 
                : cell.Value
            );
            return difference;
        }

        private int _attempt = 1;
        private bool BackupDualOdds(Cells cells, Cell cell)
        {
            try
            {
                var cellsCopy = DeepCopyCells(cells);

                var cellCopy = new Cell(
                    new CellLocation(
                        cell.Location.Row,
                        cell.Location.Column,
                        cell.Location.Block,
                        cell.Location.Index
                    )
                )
                {
                    DisplayValue = cell.DisplayValue,
                    Value = cell.Value,
                    isEnabled = cell.isEnabled,
                    hasValue = cell.hasValue,
                    isHighlighted = false,
                    CellPossibilities = new CellPossibilities
                    {
                        List = new List<int>(cell.CellPossibilities.List)
                    }
                };

                cells.List[cellCopy.Location.Index].hasBackup = true;
                DualOddsBackups.Push((cellsCopy, cellCopy));
                var alternate = cell.CellPossibilities.List.Where(p => p > 0).LastOrDefault();
                DualOddsRecord.Add((_attempt, cell.Location.Index, cell.Value, alternate));

                _attempt++;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in BackupDualOdds(...).  Error: {ex.Message}, Inner: {ex?.InnerException?.Message}");
            }
            return false;
        }

        private bool HasCorruptedOdds(Cells cells)
        {
            var corrupt = false;
            foreach (var cell in cells.List.Where(c => !c.hasValue))
            {
                if (!cell.CellPossibilities.List.Where(p => p > 0).Any())
                {
                    corrupt = true;
                    break;
                }                
            }
            return corrupt;
        }

        private bool CompleteBoard(Cells cells)
        {
            return cells.List.All(c => c.hasValue);
        }

        public bool IsBoardValid(Cells cells, bool corrupt)   
        {
            int size = Dimensions.RowSize; // Typically 9 for standard Sudoku
            int blockSize = Dimensions.BlockSize; // Typically 3 for standard Sudoku
            bool valid = true;

            // Check rows
            for (int row = 1; row <= size; row++)
            {
                var values = cells.List
                    .Where(c => c.Location.Row == row && c.hasValue)
                    .Select(c => c.Value)
                    .ToList();
                if (values.Count != values.Distinct().Count())
                    return false;
            }

            // Check columns
            for (int col = 1; col <= size; col++)
            {
                var values = cells.List
                    .Where(c => c.Location.Column == col && c.hasValue)
                    .Select(c => c.Value)
                    .ToList();
                if (values.Count != values.Distinct().Count())
                    return false;
            }

            // Check blocks
            for (int block = 1; block <= size; block++)
            {
                var values = cells.List
                    .Where(c => c.Location.Block == block && c.hasValue)
                    .Select(c => c.Value)
                    .ToList();
                if (values.Count != values.Distinct().Count())
                    return false;
            }

            return valid && !corrupt;
        }

        private bool PlaceCellValue(ref Cells cells, int index, int value)
        {
            try
            {
                if (!(cells.List[index].CellPossibilities.List.Contains(value)))
                    return false;

                var cell = cells.List[index];
                var rowCleared = !cells.List.Where(c => c.Location.Row == cell.Location.Row).Select(c => c.Value).Contains(value);
                var columnCleared = !cells.List.Where(c => c.Location.Column == cell.Location.Column).Select(c => c.Value).Contains(value);
                var blockCleared = !cells.List.Where(c => c.Location.Block == cell.Location.Block).Select(c => c.Value).Contains(value);
                if (!rowCleared || !columnCleared || !blockCleared)
                    return false;

                cells.List[index].Value = value;
                cells.List[index] = ClearCellOdds(cells.List[index]);
                cells.List[index].DisplayValue = cells.List[index].Value.ToString();
                cells.List[index].hasValue = true;

                foreach (var c in cells.List)
                    SetCellProbabilities(ref cells, c.Location.Index);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error PlaceCellValue: {ex.Message}, Inner: {ex?.InnerException?.Message}");
            }
            return false;
        }

        private void DebugInfo(Cells cells)
        {
            Console.WriteLine($"Board: {string.Join("", cells.List.Select(c => c.Value))}");
            Console.WriteLine("Updated Probabilities:");
            cells.List.Where(c => !c.hasValue).ToList().ForEach(c =>
            {
                Console.Write($"{c.Location.Index}:{string.Join("", c.CellPossibilities.List.Where(c => c > 0))},");
            });
        }

        private bool ReloadLastBackup(ref Cells cells)
        {
            var backupCell = DualOddsBackups.Pop();
            var backupRecord = DualOddsRecord.Where(r => r.Index == backupCell.Cell.Location.Index).FirstOrDefault();
            if (backupCell.Cell.hasValue)
                return false;

            var index = backupCell.Cell.Location.Index;
            cells.List = backupCell.Cells;
            cells.List[index] = backupCell.Cell;
            var value = backupRecord.AlternateValue;

            bool result = PlaceCellValue(ref cells, index, value);

            if (!result)
                return false;

            //  remove any records from the previous backup reload, if they exist
            var previousRecords = DualOddsRecord.Where(o => o.Index != index && o.Attempt > backupRecord.Attempt).ToList();
            previousRecords.ForEach(r => DualOddsRecord.Remove(r));

            var displayBackups = DualOddsBackups.Select(b => $"Index: {b.Cell.Location.Index} ");


            if (DevConfig.SudokuSettings.GamePlaySettings.SolveSettings.ShowDebugInfo)
                Console.WriteLine($"Reload cell: {cells.List[index].Location.Index}, val: {cells.List[index].Value}, Backups: {string.Join('|', displayBackups)}, Count: {displayBackups.Count()}");
            
            if (DualOddsBackups.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool FindAndPlaceDualBackup(ref Cells cells)
        {
            var cell = FindDualOdd(ref cells);

            if (cell != null)
            {
                if (cell.hasValue)
                    return false;


                if (BackupDualOdds(cells, cell))
                {
                    var index = cell.Location.Index;
                    var value = cell.CellPossibilities.List.FirstOrDefault(p => p > 0);
                    var alternate = cell.CellPossibilities.List.LastOrDefault(p => p > 0);

                    var result = PlaceCellValue(ref cells, index, value);

                    if (!result)
                        return false;

                    var displayBackups = DualOddsBackups.Select(b => $"Index: {b.Cell.Location.Index} ");

                    if (DevConfig.SudokuSettings.GamePlaySettings.SolveSettings.ShowDebugInfo)
                        Console.WriteLine($"Dual cell: {cells.List[index].Location.Index}, val: {cells.List[index].Value}, Backups: {string.Join('|', displayBackups)}, Count: {displayBackups.Count()}");
                    
                    _dualAttempt++;
                    return true;
                }
            }
            return false;
        }

        private Cell FindDualOdd(ref Cells cells)
        {
            return cells.List
                .Where(cell => cell.CellPossibilities.List.Count(p => p > 0) == 2).FirstOrDefault();
        }

        private List<List<Cell>> GetBlockRow(Cells cells, int selectBlock)
        {
            var blockRow = new List<List<Cell>>();
            try
            {
                switch (selectBlock)
                {
                    case 1:
                        {
                            blockRow.Add(cells.List.Where(c => c.Location.Block == 1).ToList());
                            blockRow.Add(cells.List.Where(c => c.Location.Block == 2).ToList());
                            blockRow.Add(cells.List.Where(c => c.Location.Block == 3).ToList());
                            break;
                        }
                    case 4:
                        {
                            blockRow.Add(cells.List.Where(c => c.Location.Block == 4).ToList());
                            blockRow.Add(cells.List.Where(c => c.Location.Block == 5).ToList());
                            blockRow.Add(cells.List.Where(c => c.Location.Block == 6).ToList());
                            break;
                        }
                    case 7:
                        {
                            blockRow.Add(cells.List.Where(c => c.Location.Block == 7).ToList());
                            blockRow.Add(cells.List.Where(c => c.Location.Block == 8).ToList());
                            blockRow.Add(cells.List.Where(c => c.Location.Block == 9).ToList());
                            break;
                        }
                }
                return blockRow;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBlockRow: {ex.Message}, Inner: {ex?.InnerException?.Message}");
                return null;
            }
        }

        private bool FindInitialRowPattern(ref List<Cell> initialRow, List<Cell> block)
        {
            var index = 1;
            foreach (var cell in block)
            {
                if (cell.Value > 0)
                    initialRow.Add(cell);
                if (initialRow.Count == 3 && index % 3 == 0)
                {
                    return true;
                }
                else if (index > 0 && index % 3 == 0)
                {
                    initialRow.Clear();
                    index = 0;
                }                
                index++;
            }
            return false;
        }

        private bool FindOpposingRowPattern(ref List<Cell> opposingRow, List<List<Cell>> blockRow)
        {
            var index = 1;
            foreach (var block in blockRow)
            {
                foreach (var cell in block)
                {
                    if (cell.Value > 0)
                        opposingRow.Add(cell);
                    if (opposingRow.Count == 3 && index % 3 == 0)
                    {
                        return true;
                    }
                    else if (index > 0 && index % 3 == 0)
                    {
                        opposingRow.Clear();
                        index = 0;
                    }
                    index++;
                }
            }
            return false;
        }

        private List<Cell> FindFilledRow(List<List<List<Cell>>> blockRows)
        {
            var found = false;
            var initialRow = new List<Cell>();
            foreach (var blockRow in blockRows)
            {
                foreach (var block in blockRow)
                {
                    found = FindInitialRowPattern(ref initialRow, block);
                    if (!found)
                        continue;
                    else
                        break;
                }
                if (found)
                    break;
            }
            return initialRow;
        }

        private List<Cell> FindOpposingRow(List<List<List<Cell>>> blockRows, int block, int row)
        {
            var opposingRow = new List<Cell>();
            var checkBlocks = blockRows.FirstOrDefault(b => b[0][0].Location.Row == row);
            if (checkBlocks == null)
                return opposingRow;

            var blockRow = checkBlocks.Where(c => c[0].Location.Block != block).ToList();

            var found = FindOpposingRowPattern(ref opposingRow, blockRow);
            if (!found)
                return opposingRow;
            
            return opposingRow;            
        }

        private int FindPatternValue(List<Cell> filled, List<Cell> opposing)
        {
            var value = 0;
            var index = 1;
            var filledValues = filled.Select(c => c.Value).ToList();
            var opposingValues = opposing.Select(c => c.Value).ToList();
            foreach (var filledValue in filledValues)
            {
                if (opposingValues.Contains(value))
                    return value;

                if (index == 3)
                    return filledValue;
                index++;
            }
            return value;
        }

        private bool FindSingleEmptyRow(ref List<Cell> finalRow, Cells cells, List<List<List<Cell>>> blockRows, List<Cell> filledRow, List<Cell> opposingRow, int value)
        {
            var initialBlock = filledRow[0].Location.Block;
            var initialRowIndex = filledRow[0].Location.Row;
            var opposingBlock = opposingRow[0].Location.Block;
            var opposingRowIndex = opposingRow[0].Location.Row;
            var finalBlock = blockRows.FirstOrDefault(b => b[0][0].Location.Row == opposingRowIndex &&
                b[0][0].Location.Block != initialBlock && b[0][0].Location.Block != opposingBlock)?.FirstOrDefault();
            if (finalBlock == null)
                return false;

            finalRow = finalBlock.Where(b => b.Location.Row == opposingRowIndex).ToList();
            if (finalRow.Where(c => !c.hasValue).Count() > 1 || finalRow.Where(c => !c.hasValue).Count() < 1)
                return false;

            return true;
        }

        private List<List<Cell>> GetBlockColumn(Cells cells, int selectColumn)
        {
            var blockColumn = new List<List<Cell>>();
            try
            {
                switch (selectColumn)
                {
                    case 1:
                        {
                            blockColumn.Add(cells.List.Where(c => c.Location.Block == 1).ToList());
                            blockColumn.Add(cells.List.Where(c => c.Location.Block == 4).ToList());
                            blockColumn.Add(cells.List.Where(c => c.Location.Block == 7).ToList());
                            break;
                        }
                    case 4:
                        {
                            blockColumn.Add(cells.List.Where(c => c.Location.Block == 2).ToList());
                            blockColumn.Add(cells.List.Where(c => c.Location.Block == 5).ToList());
                            blockColumn.Add(cells.List.Where(c => c.Location.Block == 8).ToList());
                            break;
                        }
                    case 7:
                        {
                            blockColumn.Add(cells.List.Where(c => c.Location.Block == 3).ToList());
                            blockColumn.Add(cells.List.Where(c => c.Location.Block == 6).ToList());
                            blockColumn.Add(cells.List.Where(c => c.Location.Block == 9).ToList());
                            break;
                        }
                }
                return blockColumn;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBlockColumn: {ex.Message}, Inner: {ex?.InnerException?.Message}");
                return null;
            }
        }

        private List<Cell> FindFilledColumn(List<List<List<Cell>>> blockColumns)
        {
            if (blockColumns == null || blockColumns.Count == 0)
                return null;

            var found = false;
            var initialColumn = new List<Cell>();
            foreach (var blockColumn in blockColumns)
            {
                foreach (var block in blockColumn)
                {
                    found = FindInitialColumnPattern(ref initialColumn, block); 
                    if (!found)
                        continue;
                    else
                        break;
                }
                if (found)
                    break;
            }
            return initialColumn;
        }

        private bool FindInitialColumnPattern(ref List<Cell> initialColumn, List<Cell> block)
        {
            var index = 1;
            var col = block.FirstOrDefault().Location.Column;
            for (int column = col; column <= Dimensions.ColumnSize; column++)
            {
                foreach (var cell in block.Where(b => b.Location.Column == column))
                {
                    if (cell.Value > 0)
                        initialColumn.Add(cell);
                    if (initialColumn.Count == 3 && index % 3 == 0)
                    {
                        return true;
                    }
                    else if (index > 0 && index % 3 == 0)
                    {
                        initialColumn.Clear();
                        index = 0;
                    }
                    index++;
                }
            }
            return false;
        }

        private List<Cell> FindOpposingColumn(List<List<List<Cell>>> blockColumns, int block, int column)
        {
            var opposingColumn = new List<Cell>();
            var checkBlocks = blockColumns.FirstOrDefault(b => b[0][0].Location.Column == column);
            if (checkBlocks == null)
                return opposingColumn;

            var blockColumn = checkBlocks.Where(c => c[0].Location.Block != block).ToList();

            var found = FindOpposingColumnPattern(ref opposingColumn, blockColumn);
            if (!found)
                return opposingColumn;

            return opposingColumn;
        }

        private bool FindOpposingColumnPattern(ref List<Cell> opposingColumn, List<List<Cell>> blockColumn)
        {
            var index = 1;
            foreach (var block in blockColumn)
            {
                foreach (var cell in block)
                {
                    if (cell.Value > 0)
                        opposingColumn.Add(cell);
                    if (opposingColumn.Count == 3 && index % 3 == 0)
                    {
                        return true;
                    }
                    else if (index > 0 && index % 3 == 0)
                    {
                        opposingColumn.Clear();
                        index = 0;
                    }
                    index++;
                }
            }
            return false;
        }

        private bool FindSingleEmptyColumn(ref List<Cell> finalColumn, Cells cells, List<List<List<Cell>>> blockColumns, List<Cell> filledColumn, List<Cell> opposingColumn, int value)
        {
            var initialBlock = filledColumn[0].Location.Block;
            var initialColumnIndex = filledColumn[0].Location.Column;
            var opposingBlock = opposingColumn[0].Location.Block;
            var opposingColumnIndex = opposingColumn[0].Location.Column;
            var finalBlock = blockColumns.FirstOrDefault(b => b[0][0].Location.Column == opposingColumnIndex &&
                b[0][0].Location.Block != initialBlock && b[0][0].Location.Block != opposingBlock)?.FirstOrDefault();
            if (finalBlock == null)
                return false;

            finalColumn = finalBlock.Where(b => b.Location.Column == opposingColumnIndex).ToList();
            if (finalColumn.Where(c => !c.hasValue).Count() > 1 || finalColumn.Where(c => !c.hasValue).Count() < 1)
                return false;

            return true;
        }

        private bool ProcessHighlights(ref Cells cells, int value)
        {
            for (int block = 1; block <= Dimensions.BlockSize; block++)
            {
                var found = cells.List.Where(c => c.Location.Block == block && !c.hasValue && !c.isHighlighted);
                if (found.Count() == 1)
                {
                    var index = found.FirstOrDefault().Location.Index;
                    var result = PlaceCellValue(ref cells, index, value);
                    if (!result)
                        continue;

                    if (DevConfig.SudokuSettings.GamePlaySettings.SolveSettings.ShowDebugInfo)
                        Console.WriteLine($"Highlight cell: {cells.List[index].Location.Index}, val: {cells.List[index].Value}");
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Solution Processors
        private bool ProcessRowPatterns(ref Cells cells)    
        {
            /*
                1. Get list of block rows
                2. Find block with filled row, if none, exit
                3. Find opposing block with opposing filled row, if none, exit
                4. Find first value from #2 row that isn't in #3 row, if none, exit
                5. Find single empty cell in final block, same row as #3. if none, exit
                6. Place value from #4 in single empty cell #5
            */

            //  1.Get list of block rows
            List<List<List<Cell>>> blockRows = new List<List<List<Cell>>>();
            for (int i = 1; i <= Dimensions.BlockSize; i += 3)            
                blockRows.Add(GetBlockRow(cells, i));

            //  2. Find block with filled row, if none, exit
            List<Cell> filledRow = FindFilledRow(blockRows);
            if (!filledRow.Any())
                return false;

            //  3. Find opposing block with opposing filled row, if none, exit
            List<Cell> opposingRow = FindOpposingRow(blockRows,
                filledRow[0].Location.Block,
                filledRow[0].Location.Row);
            if (!opposingRow.Any())
                return false;

            //  4. Find first value from #2 row that isn't in #3 row, if none, exit
            var value = FindPatternValue(filledRow, opposingRow);
            if (value == 0)
                return false;

            //  5. Find single empty cell in final block, same row as #3. if none, exit
            var finalRow = new List<Cell>();
            var found = FindSingleEmptyRow(ref finalRow, cells, blockRows, filledRow, opposingRow, value);
            if (!found)
                return false;

            //  6. Place value from #4 in single empty cell #5
            var result = PlaceCellValue(ref cells, finalRow.FirstOrDefault(c => !c.hasValue).Location.Index, value);
            if (!result)
                return false;

            if (DevConfig.SudokuSettings.GamePlaySettings.SolveSettings.ShowDebugInfo)
                Console.WriteLine($"Row Patterns: cell: {finalRow.FirstOrDefault(c => c.Value == value).Location.Index}, val: {value}");
            return true;
        }

        private bool ProcessColumnPatterns(ref Cells cells)
        {
            /*
                1. Get list of block columns
                2. Find block with filled column, if none, exit
                3. Find opposing block with opposing filled column, if none, exit
                4. Find first value from #2 column that isn't in #3 column, if none, exit
                5. Find single empty cell in final block, same column as #3. if none, exit
                6. Place value from #4 in single empty cell #5
            */

            //  1.Get list of block columns
            List<List<List<Cell>>> blockColumns = new List<List<List<Cell>>>();
            for (int i = 1; i <= Dimensions.ColumnSize; i += 3)
                blockColumns.Add(GetBlockColumn(cells, i));

            //  2. Find block with filled column, if none, exit
            List<Cell> filledColumn = FindFilledColumn(blockColumns);
            if (!filledColumn.Any())
                return false;

            //  3. Find opposing block with opposing filled column, if none, exit
            List<Cell> opposingColumn = FindOpposingColumn(blockColumns,
                filledColumn[0].Location.Block,
                filledColumn[0].Location.Column);
            if (!opposingColumn.Any())
                return false;

            //  4. Find first value from #2 column that isn't in #3 column, if none, exit
            var value = FindPatternValue(filledColumn, opposingColumn);
            if (value == 0)
                return false;

            //  5. Find single empty cell in final block, same column as #3. if none, exit
            var finalColumn = new List<Cell>();
            var found = FindSingleEmptyColumn(ref finalColumn, cells, blockColumns, filledColumn, opposingColumn, value);
            if (!found)
                return false;

            //  6. Place value from #4 in single empty cell #5
            var result = PlaceCellValue(ref cells, finalColumn.FirstOrDefault(c => !c.hasValue).Location.Index, value);
            if (!result)
                return false;

            if (DevConfig.SudokuSettings.GamePlaySettings.SolveSettings.ShowDebugInfo)
                Console.WriteLine($"Column Patterns: cell: {finalColumn.FirstOrDefault(c => c.Value == value).Location.Index}, val: {value}");

            return true;
        }

        private bool ProcessOdds(ref Cells cells)
        {
            var emptyCells = cells.List.Where(c  => !c.hasValue).ToList();
            var update = false;
            foreach (var cell in emptyCells)
            {
                if (cell.CellPossibilities.List.Where(p => p != 0).Count() == 1 && !cell.hasValue)
                {
                    var index = cell.Location.Index;
                    var value = cell.CellPossibilities.List.Where(p => p != 0).FirstOrDefault();
                    var result = PlaceCellValue(ref cells, index, value);
                    if (!result)
                        continue;

                    if (DevConfig.SudokuSettings.GamePlaySettings.SolveSettings.ShowDebugInfo)
                        Console.WriteLine($"Odd cell: {cell.Location.Index}, val: {cell.Value}");

                    if (!update)
                        update = true;
                } else
                {
                    continue;
                }
            }
            return update;
        }

        int _dualAttempt = 1;
        /// <summary>
        /// Attempts to solve cells with exactly two possible values ("dual odds").
        /// If reload is true, restores the previous board state and tries the alternate value (backtracking).
        /// Returns true if a change was made, false otherwise.
        /// </summary>
        private bool ProcessDualOdds(ref Cells cells, bool reload = false)
        {
            bool found = false;
            if (reload && !(DualOddsBackups.Count() == 0))
            {
                do
                {
                    found = ReloadLastBackup(ref cells);

                } while (!found && !(DualOddsBackups.Count() == 0));
            }
            else
            {
                found = FindAndPlaceDualBackup(ref cells);
            }
            return found;
        }

        private bool ProcessValueCheck(ref Cells cells)
        {
            for (int value = 1; value <= Dimensions.Size/9; value++)
            {
                cells.List.ForEach(c => c.isHighlighted = false);

                foreach (var cell in cells.List.Where(c => c.hasValue && c.Value == value))
                {
                    var row = cells.List.Where(c => c.Location.Row == cell.Location.Row).ToList();
                    foreach (var c in row)
                    {
                        c.isHighlighted = true;
                    }
                    var column = cells.List.Where(c => c.Location.Column == cell.Location.Column).ToList();
                    foreach (var c in column)
                    {
                        c.isHighlighted = true;
                    }
                    var block = cells.List.Where(c => c.Location.Block == cell.Location.Block).ToList();
                    foreach (var c in block)
                    {
                        c.isHighlighted = true;
                    }
                }
                if (ProcessHighlights(ref cells, value))
                    return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Attempts to solve the given Sudoku board using a combination of logical deduction and backtracking.
        /// 
        /// The method repeatedly applies the following strategies until the board is solved or no further progress can be made:
        /// 
        /// 1. ProcessOdds: Fills in cells that have only one possible value.
        /// 2. ProcessValueCheck: Fills in cells based on unique value placement in rows, columns, or blocks.
        /// 3. ProcessRowPatterns: Looks for a pattern in blocks on the same row:
        ///     1. Get list of block rows
        ///     2. Find block with filled row, if none, exit
        ///     3. Find opposing block with opposing filled row, if none, exit
        ///     4. Find first value from #2 row that isn't in #3 row, if none, exit
        ///     5. Find single empty cell in final block, same row as #3. if none, exit
        ///     6. Place value from #4 in single empty cell #5
        /// 5. ProcessColumnPatterns: Looks for a pattern in blocks on the same Column:
        ///     1. Get list of block columns
        ///     2. Find block with filled column, if none, exit
        ///     3. Find opposing block with opposing filled column, if none, exit
        ///     4. Find first value from #2 column that isn't in #3 column, if none, exit
        ///     5. Find single empty cell in final block, same column as #3. if none, exit
        ///     6. Place value from #4 in single empty cell #5
        /// 4. ProcessDualOdds: If stuck, tries cells with exactly two possibilities (backtracking if needed).
        /// 
        /// The process continues until all cells are filled, or max turns
        /// is reached.
        /// </summary>
        /// <param name="board">The Sudoku board to solve.</param>
        /// <returns>The solved board.</returns>
        /// configuration:  DEV.Sudoku Settings.GamePlay.SolveSettings.MaxAttempts - set max turns
        ///                     between 0 & 2,147,483,647.
        ///                 DEV.Sudoku Settings.GamePlay.SolveSettings.ShowDebugInfo - if "ON", will
        ///                     debug info in console.
        ///                 
        public Cells RunSolution(Cells board)
        {
            bool solved = false;
            int turns = 1;
            int maxTurns = DevConfig.SudokuSettings.GamePlaySettings.SolveSettings.MaxAttempts;
            bool progressMade = false;

            if (DevConfig.SudokuSettings.GamePlaySettings.SolveSettings.ShowDebugInfo)
            {
                Console.WriteLine($"Attempt: 0");
                DebugInfo(board);
            }
            else
            {
                Console.WriteLine("Debug Info: DISABLED");
            }

            while (!solved)
            {
                var previousBoard = DeepCopyCells(board);
                if (DevConfig.SudokuSettings.GamePlaySettings.SolveSettings.ShowDebugInfo)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine($"Turn: {turns}");
                    Console.WriteLine("Updates Made:");
                }
                do
                {
                    var oddsProgress = ProcessOdds(ref board);
                    var valueProgress = ProcessValueCheck(ref board);
                    var rowPatternProgress = ProcessRowPatterns(ref board);
                    var columnPatternProgress = ProcessColumnPatterns(ref board);
                    progressMade = oddsProgress || valueProgress || rowPatternProgress || columnPatternProgress;
                } while (progressMade);

                if (CompleteBoard(board))
                {
                    solved = true;
                    if (DevConfig.SudokuSettings.GamePlaySettings.SolveSettings.ShowDebugInfo)
                        Console.WriteLine($"Found Solution: Turns: {turns}");

                    continue;
                } else
                {
                    if (IsBoardValid(board, HasCorruptedOdds(board)))
                    {
                        if (!ProcessDualOdds(ref board) && IsBoardValid(board, HasCorruptedOdds(board)))
                            if (DualOddsBackups.Any())
                                ProcessDualOdds(ref board, true);
                    }
                    else
                    {
                        if (DualOddsBackups.Any())
                            ProcessDualOdds(ref board, true);
                        else
                            ProcessDualOdds(ref board);
                    }

                }

                if (DevConfig.SudokuSettings.GamePlaySettings.SolveSettings.ShowDebugInfo)
                {
                    if (!CompareBoardCells(board.List, previousBoard))
                    {
                        Console.WriteLine($"Diff : {DifferenceBoardCells(board.List, previousBoard)}");
                        DebugInfo(board);
                    }
                }
                if (turns >= maxTurns)
                {
                    if (DevConfig.SudokuSettings.GamePlaySettings.SolveSettings.ShowDebugInfo)
                        Console.WriteLine($"Max attempts reach.  Backup count: {DualOddsBackups.Count}");
                    break;
                }

                turns++;
            }
            return board;
        }
    }
}
