using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebSudoku_v0._0._7.Classes
{
    public class SudokuManager : ISudokuManager
    {
        #region Definitions
        private Stack<(List<Cell> CellsCopy, Cell DualOddsCellCopy)> DualOddsBackups = new Stack<(List<Cell>, Cell)>();
        public SudokuDimensions Dimensions { get; set; } = null;
        public DevConfiguration DevConfig { get; set; } 
        public SudokuManager(DevConfiguration devConfig)
        {
            DevConfig = devConfig;
            Dimensions = new SudokuDimensions(DevConfig.SudokuSettings.BoardDimensions.FirstOrDefault(), devConfig.SudokuSettings.BoardDimensions.LastOrDefault());
        }
        #endregion

        #region InitializationAndUpdates
        public void InitialOddsSetup(ref Cells cells, int index)
        {
            Cell cell = cells.List[index];
            cells.List[index].CellPossibilities.List.Clear();
            if (!cell.hasValue)
            {
                cells.List[index].CellPossibilities.List.AddRange(DevConfig.SudokuSettings.CellStatisticsInitial);
            } else
            {
                cells.List[index].CellPossibilities.List.AddRange(DevConfig.SudokuSettings.CellStatisticsEmpty);
            }
        }

        public Cell SetNextCell(string cellValue,  int index)
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
                cell = new Cell(cellLocation);
                cell.DisplayValue = DevConfig.SudokuSettings.GamePlaySettings.SolveSettings.CellDisplayValueType == "SPACE"
                    ? cellValue == "0"
                        ? " " : cellValue
                    : cellValue;
                cell.Value = int.TryParse(cellValue, out int value) ? value : 0;
                cell.hasValue = !string.IsNullOrEmpty(cellValue) && cell.Value != 0;
                cell.isHighlighted = cell.hasValue ? true : false;
                cell.isEnabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SetNextCell: Cell Index: {index}, error: {ex.Message}");
                cell = new Cell(new CellLocation(0, 0, 0, 0));
                cell.isEnabled = false;
                cell.hasValue = false;
            }
            return cell;
        }

        public void SetCellOdds(ref Cells cells, int index)
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

        private int SetBlock(int index)
        {
            int block = 0;
            if (index % Dimensions.ColumnSize <= 2 && index / Dimensions.RowSize <= 2)
            {
                return 1;
            }
            else if (index % Dimensions.ColumnSize <= 5 && index / Dimensions.RowSize <= 2)
            {
                return 2;
            }
            else if (index % Dimensions.ColumnSize <= 8 && index / Dimensions.RowSize <= 2)
            {
                return 3;
            }
            else if (index % Dimensions.ColumnSize <= 2 && index / Dimensions.RowSize <= 5)
            {
                return 4;
            }
            else if (index % Dimensions.ColumnSize <= 5 && index / Dimensions.RowSize <= 5)
            {
                return 5;
            }
            else if (index % Dimensions.ColumnSize <= 8 && index / Dimensions.RowSize <= 5)
            {
                return 6;
            }
            else if (index % Dimensions.ColumnSize <= 2 && index / Dimensions.RowSize <= 8)
            {
                return 7;
            }
            else if (index % Dimensions.ColumnSize <= 5 && index / Dimensions.RowSize <= 8)
            {
                return 8;
            }
            else if (index % Dimensions.ColumnSize <= 8 && index / Dimensions.RowSize <= 8)
            {
                return 9;
            }
            return block;
        }

        private int SetColumn(int index)
        {
            return index % Dimensions.ColumnSize + 1;
        }

        private int SetRow(int index)
        {
            return index / Dimensions.RowSize + 1;
        }
        #endregion

        #region SolveHelpers
        private Cell ClearCellOdds(Cell dualOddsCell)
        {
            for (int i = 0; i < dualOddsCell.CellPossibilities.List.Count; i++)
            {
                dualOddsCell.CellPossibilities.List[i] = 0;
            }
            return dualOddsCell;
        }

        private bool IsBackedUp(Cell cell)
        {
            foreach (var record in DualOddsBackups)
            {
                if (record.DualOddsCellCopy == cell)
                    return true;
            }

            return false;
        }

        private bool BackupDualOdds(Cells cells, Cell dualOddsCell)
        {
            try
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

                var dualOddsCellCopy = new Cell(
                    new CellLocation(
                        dualOddsCell.Location.Row,
                        dualOddsCell.Location.Column,
                        dualOddsCell.Location.Block,
                        dualOddsCell.Location.Index
                    )
                )
                {
                    DisplayValue = dualOddsCell.DisplayValue,
                    Value = dualOddsCell.Value,
                    isEnabled = dualOddsCell.isEnabled,
                    hasValue = dualOddsCell.hasValue,
                    isHighlighted = false,
                    CellPossibilities = new CellPossibilities
                    {
                        List = new List<int>(dualOddsCell.CellPossibilities.List)
                    }
                };

                cells.List[dualOddsCellCopy.Location.Index].hasBackup = true;

                DualOddsBackups.Push((cellsCopy, dualOddsCellCopy));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in BackupDualOdds(...).  Error: {ex.Message}, Inner: {ex?.InnerException?.Message}");
            }
            return false;
        }

        private bool QueryFullRows(ref Cells fullRow, Cells cells, int block)    
        {   
            var searchBlock = cells.List.Where(c => c.Location.Block == block).ToList();
            var row1 = searchBlock.Where(c => c.Location.Row == searchBlock[0].Location.Row && c.hasValue);
            var row2 = searchBlock.Where(c => c.Location.Row == searchBlock[3].Location.Row && c.hasValue);
            var row3 = searchBlock.Where(c => c.Location.Row == searchBlock[6].Location.Row && c.hasValue);

            if (row1.Count() != 3 || row2.Count() != 3 || row3.Count() != 3)
                return false;

            if (row1.Count() + row2.Count() == 6)
                return false;

            if (row1.Count() + row3.Count() == 6)
                return false;

            if (row2.Count() + row3.Count() == 6)
                return false;

            if (row1.Count() == 3)
                fullRow.List.AddRange(row1);
            else if (row2.Count() == 3)
                fullRow.List.AddRange(row2);
            else
                fullRow.List.AddRange(row3);

            return fullRow.List.Any() ? true : false;
        }

        private bool QueryFullCols(ref Cells fullCol, Cells cells, int block)   
        {
            var searchBlock = cells.List.Where(c => c.Location.Block == block).ToList();
            var col1 = searchBlock.Where(c => c.Location.Column == searchBlock[0].Location.Column && c.hasValue);
            var col2 = searchBlock.Where(c => c.Location.Column == searchBlock[3].Location.Column && c.hasValue);
            var col3 = searchBlock.Where(c => c.Location.Column == searchBlock[6].Location.Column && c.hasValue);

            if (col1.Count() != 3 || col2.Count() != 3 || col3.Count() != 3)
                return false;

            if (col1.Count() + col2.Count() == 6)
                return false;

            if (col1.Count() + col3.Count() == 6)
                return false;

            if (col2.Count() + col3.Count() == 6)
                return false;

            if (col1.Count() == 3)
                fullCol.List.AddRange(col1);
            else if (col2.Count() == 3)
                fullCol.List.AddRange(col2);
            else
                fullCol.List.AddRange(col3);

            return fullCol.List.Any() ? true : false;
        }

        private bool DistinctOpposing(ref int value, Cells full, Cells opposing)
        {
            var counter = 0;
            var distinct = false;
            foreach (var c in full.List)
            {
                counter = 0;
                if (opposing.List.Select(l => l.Value).ToList().Contains(c.Value))
                {
                    continue;
                }
                else
                {
                    counter++;
                    if (counter == 3)
                    {
                        distinct = true;
                        value = c.Value;
                        break;
                    }
                }
            }
            return distinct;
        }

        private bool QueryFinalColCell(ref Cell? finalCell, Cells cells, Cells fullCol, Cells opposingCol)
        {
            var blockIndex = cells.List.FirstOrDefault(c =>
                c.Location.Column == opposingCol.List[0].Location.Column
                && c.Location.Block != fullCol.List[0].Location.Block
                && c.Location.Block != opposingCol.List[0].Location.Block).Location.Block;

            var block = cells.List.Where(c => c.Location.Block == blockIndex).ToList();
            var col = block.Where(c => c.Location.Column == opposingCol.List[0].Location.Column && !c.hasValue);
            if (col == null || !col.Any())
                return false;

            if (col.Count() != 1)
                finalCell = null;
            else
                finalCell = col.FirstOrDefault(c => c.Value == 0);

            return finalCell != null ? true : false;
        }

        private bool QueryFinalRowCell(ref Cell? finalCell, Cells cells, Cells fullRow, Cells opposingRow)
        {
            var blockIndex = cells.List.FirstOrDefault(c => 
                c.Location.Row == opposingRow.List[0].Location.Row
                && c.Location.Block != fullRow.List[0].Location.Block
                && c.Location.Block != opposingRow.List[0].Location.Block).Location.Block;

            var block = cells.List.Where(c => c.Location.Block == blockIndex).ToList();
            var row = block.Where(c => c.Location.Row == opposingRow.List[0].Location.Row && !c.hasValue);
            if (row == null || !row.Any())
                return false;

            if (row.Count() != 1)
                finalCell = null;
            else
                finalCell = row.FirstOrDefault(c => c.Value == 0);

                return finalCell != null ? true : false;
        }

        private bool PossibleValue(Cell? finalCell, int value)
        {
            if (finalCell == null) return false;

            if (finalCell.CellPossibilities.List.Contains(value))
                return true;

            return false;
        }

        public bool IsBoardValid(Cells cells)
        {
            int size = Dimensions.RowSize; // Typically 9 for standard Sudoku
            int blockSize = Dimensions.BlockSize; // Typically 3 for standard Sudoku

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

            return true;
        }
        #endregion

        #region SolveProcessors
        private bool ProcessPatterns(ref Cells cells)
        {
            for (int blk = 1; blk <= DevConfig.SudokuSettings.BoardDimensions.LastOrDefault(); blk++)
            {
                Cells fullRow = new Cells();
                if (!QueryFullRows(ref fullRow, cells, blk))
                    return false;

                Cells opposingRow = new Cells();
                if (!QueryFullRows(ref opposingRow, cells, fullRow.List[0].Location.Block))
                    return false;

                int value = 0;
                if (!DistinctOpposing(ref value, fullRow, opposingRow))
                    return false;

                Cell? finalCell = new Cell(new CellLocation(0,0,0,0));
                if (!QueryFinalRowCell(ref finalCell, cells, fullRow, opposingRow))
                    return false;

                if (finalCell.hasValue)
                    return false;

                if (!PossibleValue(finalCell, value))
                    return false;

                var index = finalCell.Location.Index;
                cells.List[index].Value = value;
                cells.List[index].DisplayValue = value.ToString();
                cells.List[index].hasValue = true;
                cells.List[index] = ClearCellOdds(cells.List[index]);

                for (int i = 0; i < cells.List.Count(); i++)
                    SetCellOdds(ref cells, cells.List[index].Location.Index);

                Console.WriteLine($"Pattern Row: {cells.List[index].Location.Index}, val: {cells.List[index].Value}");
                return true;
            }

            for (int blk = 1; blk <= DevConfig.SudokuSettings.BoardDimensions.LastOrDefault(); blk++)
            {
                Cells fullCol = new Cells();
                if (!QueryFullCols(ref fullCol, cells, blk))
                    return false;

                Cells opposingCol = new Cells();
                if (!QueryFullCols(ref opposingCol, cells, fullCol.List[0].Location.Block))
                    return false;

                int value = 0;
                if (!DistinctOpposing(ref value, fullCol, opposingCol))
                    return false;

                Cell? finalCell = new Cell(new CellLocation(0, 0, 0, 0));
                if (!QueryFinalColCell(ref finalCell, cells, fullCol, opposingCol))
                    return false;

                if (finalCell.hasValue)
                    return false;

                if (!PossibleValue(finalCell, value))
                    return false;

                var index = finalCell.Location.Index;
                cells.List[index].Value = value;
                cells.List[index].DisplayValue = value.ToString();
                cells.List[index].hasValue = true;
                cells.List[index] = ClearCellOdds(cells.List[index]);

                for (int i = 0; i < cells.List.Count(); i++)
                    SetCellOdds(ref cells, cells.List[index].Location.Index);

                Console.WriteLine($"Pattern Col: {cells.List[index].Location.Index}, val: {cells.List[index].Value}");
                return true;
            }
            return false;
        }

        private bool ProcessOdds(ref Cells cells)
        {
            foreach (var cell in cells.List)
            {
                if (!cell.hasValue)
                {
                    if (cell.CellPossibilities.List.Where(p => p != 0).Count() == 1 && !cell.hasValue)
                    {
                        cell.Value = cell.CellPossibilities.List.Where(p => p != 0).FirstOrDefault();
                        cell.DisplayValue = cell.Value.ToString();
                        cell.hasValue = true;
                        cells.List[cell.Location.Index] = cell;
                        var oCells = cells;
                        cells.List.ForEach(c => SetCellOdds(ref oCells, cell.Location.Index));                        
                        Console.WriteLine($"Odd cell: {cell.Location.Index}, val: {cell.Value}");
                        return true;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Attempts to solve cells with exactly two possible values ("dual odds").
        /// If reload is true, restores the previous board state and tries the alternate value (backtracking).
        /// Returns true if a change was made, false otherwise.
        /// </summary>
        private bool ProcessDualOdds(ref Cells cells, bool reload = false)
        {
            if (reload && !(DualOddsBackups.Count() == 0))
            {
                var lastBackupCell = DualOddsBackups.Pop();

                if (lastBackupCell.DualOddsCellCopy.hasValue)
                    return false;

                var index = lastBackupCell.DualOddsCellCopy.Location.Index;
                cells.List = lastBackupCell.CellsCopy;
                cells.List[index] = lastBackupCell.DualOddsCellCopy;
                cells.List[index].Value = lastBackupCell.DualOddsCellCopy.CellPossibilities.List.LastOrDefault(p => p > 0);
                cells.List[index] = ClearCellOdds(cells.List[index]);
                cells.List[index].DisplayValue = cells.List[index].Value.ToString();
                cells.List[index].hasValue = true;

                foreach (var cell in cells.List)
                    SetCellOdds(ref cells, lastBackupCell.DualOddsCellCopy.Location.Index);

                Console.WriteLine($"Reload cell: {cells.List[index].Location.Index}, val: {cells.List[index].Value}");
                return true;
            }
            else
            {
                var dualOddsCell = cells.List
                    .Where(cell => cell.CellPossibilities.List.Count(p => p > 0) == 2).ToList();

                if (dualOddsCell != null && dualOddsCell.Any())
                {
                    for (int i = 0; i < dualOddsCell.Count; i++)
                    {
                        if (dualOddsCell[i].hasValue)
                            continue;

                        if (!IsBackedUp(dualOddsCell[i]))
                        {
                            if (BackupDualOdds(cells, dualOddsCell[i]))
                            {
                                var index = dualOddsCell[i].Location.Index;
                                var value = dualOddsCell[i].CellPossibilities.List.FirstOrDefault(p => p > 0);
                                cells.List[index] = ClearCellOdds(dualOddsCell[i]);
                                cells.List[index].Value = value;
                                cells.List[index].DisplayValue = value.ToString();
                                cells.List[index].hasValue = true;

                                foreach (var cell in cells.List)
                                    SetCellOdds(ref cells, cell.Location.Index);

                                Console.WriteLine($"Dual cell: {cells.List[index].Location.Index}, val: {cells.List[index].Value}");
                                return true;
                            }
                        }
                    }
                    return false;
                }
            }
            return false;
        }

        private bool ProcessValueCheck(ref Cells cells)
        {
            for (int val = 1; val <= DevConfig.SudokuSettings.BoardDimensions.LastOrDefault(); val++)
            {
                cells.List.ForEach(c => c.isHighlighted = false);
                cells.List.ForEach(c => { if (c.hasValue) c.isHighlighted = true; });

                foreach (var cell in cells.List.Where(c => c.hasValue))
                {
                    if (cell.Value == val)
                    {
                        var row = cells.List.Where(c => c.Location.Row == cell.Location.Row).ToList();
                        foreach(var c in row) { 
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
                }
                if (ProcessHighlights(ref cells, val)) { return true; }
            }
            return false;
        }

        private bool ProcessHighlights(ref Cells cells, int value)  
        {
            for (int blk = 1; blk <= DevConfig.SudokuSettings.BoardDimensions.LastOrDefault(); blk++)
            {
                var block = cells.List.Where(c => c.Location.Block == blk && !c.hasValue && !c.isHighlighted);
                if (block.Count() == 1)
                {
                    var index = block.FirstOrDefault().Location.Index;
                    cells.List[index].Value = value;
                    cells.List[index].DisplayValue = value.ToString();
                    cells.List[index].hasValue = true;
                    cells.List[index] = ClearCellOdds(cells.List[index]);

                    foreach (var cell in cells.List)
                    {
                        SetCellOdds(ref cells, cell.Location.Index);
                    }
                    Console.WriteLine($"Highlight cell: {cells.List[index].Location.Index}, val: {cells.List[index].Value}");
                    return true;
                }                
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Attempts to solve the given Sudoku board using a combination of logical deduction and backtracking.
        /// 
        /// The method repeatedly applies the following strategies until the board is solved or no further progress can be made:
        /// 1. ProcessOdds: Fills in cells that have only one possible value.
        /// 2. ProcessValueCheck: Fills in cells based on unique value placement in rows, columns, or blocks.
        /// 3. ProcessDualOdds: If stuck, tries cells with exactly two possibilities (backtracking if needed).
        /// 
        /// The process continues until all cells are filled or no more moves are possible.
        /// </summary>
        /// <param name="board">The Sudoku board to solve.</param>
        /// <returns>The solved or partially solved board.</returns>
        public Cells RunSolution(Cells board)
        {
            bool solved = false;
            while (!solved)
            {
                var oddSolutionFound = ProcessOdds(ref board);
                                
                var valueSolutionFound = ProcessValueCheck(ref board);

                if (!oddSolutionFound && !valueSolutionFound)
                {
                    var patternSolutionFound = ProcessPatterns(ref board);

                    if (!patternSolutionFound)
                    {
                        if (ProcessDualOdds(ref board))
                        {
                            continue;
                        }
                        else
                        {
                            if (ProcessDualOdds(ref board, true))
                            {
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                solved = board.List.All(c => c.hasValue) && IsBoardValid(board);
                if (!solved && !IsBoardValid(board))
                    throw new Exception($"The solver finished, but the solution was invalid.");
            }
            return board;
        }
    }
}
