namespace Web_Core_SudokuSolver_v0._0._1.Classes
{
    public interface ISudokuBoard
    {
        ISudokuManager SudokuManager { get; set; }
        Cells Cells { get; set; }
        ISudokuDimensions Dimensions { get; set; }
        string CurrentStartingPuzzle { get; set; }

    }
}
