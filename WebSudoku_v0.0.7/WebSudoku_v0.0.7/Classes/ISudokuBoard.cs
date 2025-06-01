namespace WebSudoku_v0._0._7.Classes
{
    public interface ISudokuBoard
    {
        ISudokuManager SudokuManager { get; set; }
        ISudokuDimensions Dimensions { get; set; }
        Cells GetCells();
        void createSudokuBoard(string puzzle);
        void InitializeProbabilities();
    }
}
