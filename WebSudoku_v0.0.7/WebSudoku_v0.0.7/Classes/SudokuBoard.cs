﻿using WebSudoku_v0._0._7.Configuration;

namespace WebSudoku_v0._0._7.Classes
{
    public class SudokuBoard : ISudokuBoard   
    {
        public ISudokuManager SudokuManager { get; set; } = null;
        public Cells Cells { get; set; } = new Cells();
        public ISudokuDimensions Dimensions { get; set; } = new SudokuDimensions(0, 0);

        private readonly DevConfiguration _devConfig;
        public SudokuBoard(DevConfiguration devConfig)
        {
            _devConfig = devConfig;
        }

        public Cells GetCells()
        {
            return Cells;
        }

        public void InitializeProbabilities()
        {
            var Cells = GetCells();
            for (int i = 0; i < Cells.List.Count; i++)
            {
                SudokuManager.SetupProbabilities(ref Cells, i);
                SudokuManager.SetCellProbabilities(ref Cells, i);
            }
        }

        public void createSudokuBoard(string puzzle)
        {
            this.Cells.List.Clear();
            SudokuManager = new SudokuManager(_devConfig);
            
            var dims = _devConfig.SudokuSettings.BoardDimensions;
            Dimensions = new SudokuDimensions(dims.FirstOrDefault(), dims.LastOrDefault());
            
            int index = 0;
            foreach (var cellValue in puzzle.ToCharArray())
            {
                this.Cells.List.Add(SudokuManager.createNextCell(cellValue.ToString(), index));
                index++;
            }
        }
    }
}
