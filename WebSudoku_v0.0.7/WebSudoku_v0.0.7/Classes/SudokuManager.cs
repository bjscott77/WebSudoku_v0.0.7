using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace WebSudoku_v0._0._7.Classes
{
    public class SudokuManager : ISudokuManager
    {
        #region Definitions
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

        #region InitializationAndUpdates
        public void InitialOddsSetup(ref Cells cells, int index)
        {
            if (index < 0 || index > 80)
                throw new ArgumentOutOfRangeException("index", $"Index must be between {0}:{Dimensions.Size - 1} inclusivley.  Expected: 0-80, Received: {index}");

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
                Console.WriteLine($"Error in InitialOddsSetup: {ex.Message}");
                
                Cell cell = new Cell(new CellLocation(0, 0, 0, 0));
                cell.isEnabled = false;
                cell.hasValue = false;
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
            if (index < 0 || index > 80)
                return -1;
            // Rows, columns, and blocks are 1-based
            int row = SetRow(index);    // 1-based row
            int col = SetColumn(index); // 1-based column
            int blockRow = (row - 1) / 3;
            int blockCol = (col - 1) / 3;
            return (blockRow * 3) + blockCol + 1;
        }

        private int SetColumn(int index)
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

        #region SolveHelpers
        private Cell ClearCellOdds(Cell cell)
        {
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

        private int _attempt = 1;
        private bool BackupDualOdds(Cells cells, Cell dualCell)
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
                        dualCell.Location.Row,
                        dualCell.Location.Column,
                        dualCell.Location.Block,
                        dualCell.Location.Index
                    )
                )
                {
                    DisplayValue = dualCell.DisplayValue,
                    Value = dualCell.Value,
                    isEnabled = dualCell.isEnabled,
                    hasValue = dualCell.hasValue,
                    isHighlighted = false,
                    CellPossibilities = new CellPossibilities
                    {
                        List = new List<int>(dualCell.CellPossibilities.List)
                    }
                };

                cells.List[dualOddsCellCopy.Location.Index].hasBackup = true;
                DualOddsBackups.Push((cellsCopy, dualOddsCellCopy));
                var altValue = dualCell.CellPossibilities.List.Where(p => p > 0).LastOrDefault();
                DualOddsRecord.Add((_attempt, dualCell.Location.Index, dualCell.Value, altValue));

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
            foreach (var cell in cells.List.Where(c => !c.hasValue))
            {
                if (cell.CellPossibilities.List.Where(p => p > 0).Count() == 0)
                {
                    return true;
                }               
            }
            return false;
        }

        private bool CompleteBoard(Cells cells)
        {
            return cells.List.Where(c => c.hasValue).Count() == 81;
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
                cells.List[index].Value = value;
                cells.List[index] = ClearCellOdds(cells.List[index]);
                cells.List[index].DisplayValue = cells.List[index].Value.ToString();
                cells.List[index].hasValue = true;

                foreach (var cell in cells.List)
                    SetCellOdds(ref cells, index);

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
            cells.List.Where(c => !c.hasValue).ToList().ForEach(c =>
            {
                Console.WriteLine($"Cell: {c.Location.Index}, Value: {c.Value}, Odds: {string.Join("", c.CellPossibilities.List.Where(c => c > 0))}");
            });
        }

        #endregion

        #region SolveProcessors
        private bool ProcessPatterns(ref Cells cells)
        {
            return false;
        }

        private bool ProcessOdds(ref Cells cells)
        {
            var emptyCells = cells.List.Where(c  => !c.hasValue).ToList();
            foreach (var cell in emptyCells)
            {
                if (cell.CellPossibilities.List.Where(p => p != 0).Count() == 1 && !cell.hasValue)
                {
                    var index = cell.Location.Index;
                    var value = cell.CellPossibilities.List.Where(p => p != 0).FirstOrDefault();
                    var result = PlaceCellValue(ref cells, index, value);
                    if (!result)
                        throw new Exception($"Placing Cell {index} failed @ ProcessOdds");

                    Console.WriteLine($"Odd cell: {cell.Location.Index}, val: {cell.Value}");
                    return true;
                }
                else
                {
                    continue;
                }
            }
            return false;
        }

        int _dualAttempt = 1;
        /// <summary>
        /// Attempts to solve cells with exactly two possible values ("dual odds").
        /// If reload is true, restores the previous board state and tries the alternate value (backtracking).
        /// Returns true if a change was made, false otherwise.
        /// </summary>
        private bool ProcessDualOdds(ref Cells cells, bool reload = false)
        {
            if (reload && !(DualOddsBackups.Count() == 0))
            {
                bool found = false;
                do
                {
                    var backupCell = DualOddsBackups.Pop();
                    var backupRecord = DualOddsRecord.Where(r => r.Index == backupCell.Cell.Location.Index).FirstOrDefault();
                    if (backupCell.Cell.hasValue)
                        throw new Exception($"Backed up cells should never contain a value");

                    var index = backupCell.Cell.Location.Index;
                    cells.List = backupCell.Cells;
                    cells.List[index] = backupCell.Cell;
                    var value = backupRecord.AlternateValue;
                    bool result = PlaceCellValue(ref cells, index, value);
                    if (!result)
                        throw new Exception($"Placing Cell {index} failed @ ProcessDualOdds");

                    //  remove any records from the previous backup reload, if they exist
                    var previousRecords = DualOddsRecord.Where(o => o.Index != index && o.Attempt > backupRecord.Attempt).ToList();
                    previousRecords.ForEach(r => DualOddsRecord.Remove(r));

                    Console.WriteLine($"Reload cell: {cells.List[index].Location.Index}, val: {cells.List[index].Value}");
                    if (DualOddsBackups.Count == 0)
                    {
                        found = false;
                        break;
                    }
                    else
                    {
                        found = true;
                    }

                } while (!found);
                return found;
            }
            else
            {
                bool updated = false;
                var cell = cells.List
                    .Where(cell => cell.CellPossibilities.List.Count(p => p > 0) == 2).FirstOrDefault();

                if (cell != null)
                {
                    if (cell.hasValue)
                        throw new Exception($"Cells with a value should not have any odds defined.");

                    if (!IsBackedUp(cell))
                    {
                        if (BackupDualOdds(cells, cell))
                        {
                            var index = cell.Location.Index;
                            var value = cell.CellPossibilities.List.FirstOrDefault(p => p > 0);
                            var alternate = cell.CellPossibilities.List.LastOrDefault(p => p > 0);
                            var result = PlaceCellValue(ref cells, index, value);
                            if (!result)
                                throw new Exception($"Placing Cell {index} failed @ ProcessDualOdds Reset");

                            Console.WriteLine($"Dual cell: {cells.List[index].Location.Index}, val: {cells.List[index].Value}");
                            _dualAttempt++;
                            updated = true;
                        }
                    }
                }
                return updated;
            }
        }

        private bool ProcessValueCheck(ref Cells cells)
        {
            for (int value = 1; value <= DevConfig.SudokuSettings.BoardDimensions.LastOrDefault(); value++)
            {
                cells.List.ForEach(c => c.isHighlighted = false);

                foreach (var cell in cells.List.Where(c => c.hasValue))
                {
                    if (cell.Value == value)
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
                if (ProcessHighlights(ref cells, value))
                    return true;
            }
            return false;
        }

        private bool ProcessHighlights(ref Cells cells, int value)  
        {
            for (int block = 1; block <= DevConfig.SudokuSettings.BoardDimensions.LastOrDefault(); block++)
            {
                var found = cells.List.Where(c => c.Location.Block == block && !c.hasValue && !c.isHighlighted);
                if (found.Count() == 1)
                {
                    var index = found.FirstOrDefault().Location.Index;
                    var result = PlaceCellValue(ref cells, index, value);
                    if (!result)
                        throw new Exception($"Placing Cell {index} failed @ ProcessHighlights");

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
            int attempts = 0;
            int maxattempts = 10000;
            bool progressMade = false;
            while (!solved)
            {
                do
                {
                    progressMade = ProcessOdds(ref board);
                    progressMade = ProcessValueCheck(ref board);
                } while (progressMade);

                if (IsBoardValid(board, !HasCorruptedOdds(board)))
                {
                    ProcessDualOdds(ref board);
                }
                else
                {
                    //  Inalid board, reset to an earlier state if backups exist,
                    //  otherwise look for another dual odd.
                    if (DualOddsBackups.Any())
                    {
                        ProcessDualOdds(ref board, true);
                    } else
                    {
                        ProcessDualOdds(ref board);
                    }
                }

                DebugInfo(board);

                solved = board.List.All(c => c.hasValue);

                if (attempts == maxattempts)
                {
                    Console.WriteLine($"Max attempts reach.  Backup count: {DualOddsBackups.Count}");
                    break;
                }

                attempts++;
            }
            return board;
        }
    }
}
