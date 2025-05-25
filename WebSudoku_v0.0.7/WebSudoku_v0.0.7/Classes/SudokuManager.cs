using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebSudoku_v0._0._7.Classes
{
    public class SudokuManager : ISudokuManager
    {
        public SudokuDimensions Dimensions { get; set; } = null;
        public DevConfiguration DevConfig { get; set; } 
        public SudokuManager(DevConfiguration devConfig)
        {
            DevConfig = devConfig;
            Dimensions = new SudokuDimensions(DevConfig.SudokuSettings.BoardDimensions.FirstOrDefault(), devConfig.SudokuSettings.BoardDimensions.LastOrDefault());
        }

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

        //  REM: Complete Pattern Checking
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

        public Cells RunSolution(Cells board)
        {
            bool solved = false;
            while (!solved)
            {
                //  Check for cell solutions based on current odds
                var oddSolutionFound = ProcessOdds(ref board);

                //  Check for cell solutions based on current board values and update odds
                var valueSolutionFound = ProcessValueCheck(ref board);

                solved = board.List.All(c => c.hasValue);

                if (!oddSolutionFound && !valueSolutionFound) break;
            }
            return board;
        }
    }
}
