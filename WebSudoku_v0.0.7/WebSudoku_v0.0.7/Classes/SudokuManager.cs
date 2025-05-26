using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebSudoku_v0._0._7.Classes
{
    public class SudokuManager : ISudokuManager
    {
        private Stack<(List<Cell> CellsCopy, Cell DualOddsCellCopy)> DualOddsBackups = new Stack<(List<Cell>, Cell)>();
        public SudokuDimensions Dimensions { get; set; } = null;
        public DevConfiguration DevConfig { get; set; } 
        public SudokuManager(DevConfiguration devConfig)
        {
            DevConfig = devConfig;
            Dimensions = new SudokuDimensions(DevConfig.SudokuSettings.BoardDimensions.FirstOrDefault(), devConfig.SudokuSettings.BoardDimensions.LastOrDefault());
        }
        public int CurrentRound { get; set; } = 0;
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

        private bool ProcessOdds(ref Cells cells)
        {
            foreach (var cell in cells.List)
            {
                if (cell.hasValue)
                {
                    cell.isHighlighted = true;
                } else
                {
                    if (cell.CellPossibilities.List.Where(p => p != 0).Count() == 1)
                    {
                        cell.Value = cell.CellPossibilities.List.Where(p => p != 0).FirstOrDefault();
                        cell.DisplayValue = cell.Value.ToString();
                        cell.hasValue = true;
                        cell.isHighlighted = true;
                        cells.List[cell.Location.Index] = cell;
                        var oCells = cells;
                        cells.List.ForEach(c => SetCellOdds(ref oCells, cell.Location.Index));                        
                        Console.WriteLine($"ProcessOdds Solved - Index: {cell.Location.Index}={cell.Value}");
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
            if (reload)
            {
                // Backtracking: restore the last backup state and try the alternate value
                var lastBackupCell = DualOddsBackups.Pop();
                var indx = lastBackupCell.DualOddsCellCopy.Location.Index;

                // Restore the entire board and the specific dual-odds cell
                cells.List = lastBackupCell.CellsCopy;
                cells.List[indx] = lastBackupCell.DualOddsCellCopy;

                // Set the cell to the alternate value (the other possibility)
                cells.List[indx].Value = lastBackupCell.DualOddsCellCopy.CellPossibilities.List.LastOrDefault(p => p > 0);
                cells.List[indx] = ClearCellOdds(cells.List[indx]);
                cells.List[indx].DisplayValue = cells.List[indx].Value.ToString();
                cells.List[indx].hasValue = true;

                // Update odds for all cells based on the new value
                foreach (var cell in cells.List)
                {
                    SetCellOdds(ref cells, lastBackupCell.DualOddsCellCopy.Location.Index);
                }

                // Debug output
                Console.WriteLine($"Cell Values {string.Join(' ', cells.List.Select(c => c.Value))}");
                Console.WriteLine($"Backup Cell: Index: {cells.List[lastBackupCell.DualOddsCellCopy.Location.Index].Location.Index}");
                Console.WriteLine($"Backup Cell: Value: {cells.List[lastBackupCell.DualOddsCellCopy.Location.Index].Value}");

                return true;
            }
            else
            {
                // Find all cells with exactly two possible values (dual odds)
                var dualOddsCell = cells.List
                    .Where(cell => cell.CellPossibilities.List.Count(p => p > 0) == 2).ToList();

                if (dualOddsCell != null)
                {
                    for (int i = 0; i < dualOddsCell.Count; i++)
                    {
                        // Only process cells that haven't been backed up yet
                        if (!IsBackedUp(dualOddsCell[i]))
                        {
                            // Backup the current board state and the dual-odds cell
                            if (BackupDualOdds(cells, dualOddsCell[i]))
                            {
                                var indx = dualOddsCell[i].Location.Index;
                                // Pick the first possible value and set it
                                var value = dualOddsCell[i].CellPossibilities.List.FirstOrDefault(p => p > 0);
                                dualOddsCell[i] = ClearCellOdds(dualOddsCell[i]);
                                cells.List[indx].Value = value;
                                cells.List[indx].DisplayValue = value.ToString();
                                cells.List[indx].hasValue = true;

                                // Debug output
                                Console.WriteLine($"Cell Possibilities: {string.Join(' ', dualOddsCell[i].CellPossibilities.List)}");
                                Console.WriteLine($"ProcessDualOdds Attempt 1 - Index: {indx}={value}");

                                // Update odds for all cells based on the new value
                                foreach (var cell in cells.List)
                                {
                                    SetCellOdds(ref cells, dualOddsCell[i].Location.Index);
                                }
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
                    var indx = block.FirstOrDefault().Location.Index;
                    Console.WriteLine($"ProcessHighlights Solved - Index: {indx}={value}");
                    cells.List[indx].Value = value;
                    cells.List[indx].DisplayValue = value.ToString();
                    cells.List[indx].hasValue = true;

                    foreach (var cell in cells.List)
                    {
                        SetCellOdds(ref cells, cell.Location.Index);
                    }
                    return true;
                }                
            }
            return false;
        }

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
                CurrentRound++;
                //  Check for cell solutions based on current odds
                var oddSolutionFound = ProcessOdds(ref board);

                //  Check for cell solutions based on current board values and update odds
                var valueSolutionFound = ProcessValueCheck(ref board);

                solved = board.List.All(c => c.hasValue);

                if (!oddSolutionFound && !valueSolutionFound)
                {
                    if (ProcessDualOdds(ref board))
                    {

                    } else
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
            return board;
        }
    }
}
