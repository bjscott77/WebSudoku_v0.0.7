namespace WebSudoku_v0._0._7.Classes
{
    public interface ISudokuBoard
    {
        ISudokuManager SudokuManager { get; set; }
        Cells Cells { get; set; }
        ISudokuDimensions Dimensions { get; set; }
        void InitializeBoard(string puzzle);
        void InitializeOdds();

    }
}
