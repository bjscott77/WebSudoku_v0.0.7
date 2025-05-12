namespace Web_Core_SudokuSolver_v0._0._1.Classes
{
    public interface ISudokuManager
    {
        Cell SetNextCell(string puzzle, int index);

        /// <summary>
        /// Processes one round of the Sudoku solving algorithm.
        /// </summary>
        /// <param name="board"></param>
        /// <returns>True if one or more cells have been solved</returns>
        bool ProcessRound(string board);   
    }
}
