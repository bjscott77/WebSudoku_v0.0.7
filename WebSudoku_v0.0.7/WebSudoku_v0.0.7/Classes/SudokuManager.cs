using System;
using System.Linq;
using System.Reflection;

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

        public Cells InitialOddsSetup(Cells cells, int index)
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
                return cells;
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
                cell.DisplayValue = cellValue;
                cell.Value = int.TryParse(cellValue, out int value) ? value : 0;
                cell.hasValue = !string.IsNullOrEmpty(cellValue) && cell.Value != 0;
                cell.isEnabled = value > 0 ?  false: true;
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

        public Cells SetCellOdds(Cells cells, int index)
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
            return cells;
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

        public Cells ProcessOdds(Cells cells)
        {
            Cells emptyCells = new Cells();
            emptyCells.List = cells.List.Where(c => !c.hasValue).ToList();
            foreach (var cell in emptyCells.List)
            {
                if (cell.CellPossibilities.List.Where(p => p != 0).Count() == 1)
                {
                    cell.Value = cell.CellPossibilities.List.Where(p => p != 0).FirstOrDefault();
                    cell.DisplayValue = cell.Value.ToString();
                    cell.hasValue = true;
                    //cell.isEnabled = false;
                    cells.List[cell.Location.Index] = cell;
                    break;
                } else
                {
                    continue;
                }
            }
            return cells;
        }

        //  REM: Complete HiLite and Pattern Checking
        public Cells ProcessValueCheck(Cells cells)
        {
            Cells emptyCells = new Cells();
            emptyCells.List = cells.List.Where(c => !c.hasValue).ToList();
            //Check Rows for 1-9 values
            //Check Columns for 1-9 values
            //Check Blocks for 1-9 values
            return cells;
        }

        public Cells RunSolution(Cells board)
        {
            bool solved = false;
            int attempt = 1;
            int maxAttempts = 1000;
            while (!solved)
            {
                board = ProcessOdds(board);
                board.List.ForEach(c => SetCellOdds(board, c.Location.Index));

                solved = board.List.All(c => c.hasValue);
                if (attempt >= maxAttempts)
                {
                    break;
                }
                attempt++;
            }
            return board;
        }
    }
}
