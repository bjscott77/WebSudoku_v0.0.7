namespace WebSudoku_v0._0._7.Classes
{
    public interface ISudokuManager
    {
        Cells InitialOddsSetup(Cells cells, int index);

        Cell SetNextCell(string puzzle, int index);

        Cells SetCellOdds(Cells cells, int index);

        /// <summary>
        /// Processes one round of the Sudoku solving algorithm.
        /// </summary>
        /// <param name="board"></param>
        /// <returns>True if one or more cells have been solved</returns>
        Cells ProcessValueCheck(Cells cells);

        /// <summary>
        /// Processes the odds of the Sudoku solving algorithm.
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        Cells ProcessOdds(Cells cells);

        Cells RunSolution(Cells board);
    }
}
