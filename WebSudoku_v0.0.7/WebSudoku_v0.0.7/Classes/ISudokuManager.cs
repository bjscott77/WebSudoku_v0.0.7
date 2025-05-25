namespace WebSudoku_v0._0._7.Classes
{
    public interface ISudokuManager
    {
        void InitialOddsSetup(ref Cells cells, int index);

        Cell SetNextCell(string puzzle, int index);

        void SetCellOdds(ref Cells cells, int index);

        Cells RunSolution(Cells board);  
    }
}
