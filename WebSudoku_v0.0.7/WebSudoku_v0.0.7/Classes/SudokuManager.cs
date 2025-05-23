﻿using System;
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
                cell.DisplayValue = DevConfig.SudokuSettings.GamePlaySettings.SolveSettings.CellDisplayValueType == "SPACE"
                    ? cellValue == "0"
                        ? " " : cellValue
                    : cellValue;
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

        //  REM: Complete Pattern Checking
        public Cells ProcessValueCheck(Cells cells)
        {
            Cells emptyCells = new Cells();
            emptyCells.List = cells.List.Where(c => !c.hasValue).ToList();

            emptyCells.List.ForEach(c => ProcessRow(c, ref cells));
            emptyCells.List.ForEach(c => ProcessCol(c, ref cells));
            emptyCells.List.ForEach(c => ProcessBlock(c, ref cells));  

            return cells;
        }

        private void ProcessRow(Cell cell, ref Cells cells)
        {
            for (int i = 1; i <= 9; i++)
            {
                SetRowHiLite(ref cells, cell, i);
            }
        }

        private void ProcessCol(Cell cell, ref Cells cells)    
        {
            for (int i = 1; i <= 9; i++)
            {
                SetColHiLite(ref cells, cell, i);
            }
        }

        private void ProcessBlock(Cell cell, ref Cells cells)  
        {
            for (int i = 1; i <= 9; i++)
            {
                SetBlockHiLite(ref cells, cell, i); 
            }
        }

        private void SetRowHiLite(ref Cells cells, Cell cell, int searchValue)
        {
            var row = cells.List.Where(c => c.Location.Row == cell.Location.Row && c.Location.Index != cell.Location.Index).ToList();
            foreach (Cell rowCell in row)
            {
                if (rowCell.Value == searchValue)
                {
                    foreach(Cell hCell in row)
                    {
                        cells.List[hCell.Location.Index].isHighlighted = true;
                        break;
                    }
                }
            }
            
        }

        private void SetColHiLite(ref Cells cells, Cell cell, int searchValue)  
        {
            var col = cells.List.Where(c => c.Location.Column == cell.Location.Column && c.Location.Index != cell.Location.Index).ToList();
            foreach (Cell colCell in col)
            {
                if (colCell.Value == searchValue)
                {
                    foreach (Cell hCell in col)
                    {
                        cells.List[hCell.Location.Index].isHighlighted = true;
                        break;
                    }
                }
            }

        }

        private void SetBlockHiLite(ref Cells cells, Cell cell, int searchValue)    
        {
            var block = cells.List.Where(c => c.Location.Block == cell.Location.Block && c.Location.Index != cell.Location.Index).ToList();
            foreach (Cell blockCell in block)
            {
                if (blockCell.Value == searchValue)
                {
                    foreach (Cell hCell in block)
                    {
                        cells.List[hCell.Location.Index].isHighlighted = true;
                        break;
                    }
                }
            }
            ProcessResult(ref cells, ref cell, searchValue);
        }

        private void ProcessResult(ref Cells cells, ref Cell cell, int value)
        {
            var cellBlock = cell.Location.Block;    
            var block = cells.List.Where(c => c.Location.Block == cellBlock && c.isHighlighted == false);

            if (block.ToList().Count == 1 && block.FirstOrDefault().Location.Index == cell.Location.Index)
                cell.Value = value;

            cells.List.ForEach(c => c.isHighlighted = false);
        }
        
        public Cells RunSolution(Cells board)
        {
            bool solved = false;
            int attempt = 1;
            int maxAttempts = 1000;
            while (!solved)
            {
                //  Check for cell solutions based on current odds
                board = ProcessOdds(board);
                board.List.ForEach(c => SetCellOdds(board, c.Location.Index));

                //  Check for cell solutions based on current board values
                board = ProcessValueCheck(board);
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
