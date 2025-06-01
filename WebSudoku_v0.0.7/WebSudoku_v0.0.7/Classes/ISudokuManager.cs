namespace WebSudoku_v0._0._7.Classes
{
    public interface ISudokuManager
    {
        void SetupProbabilities(ref Cells cells, int index);

        Cell createNextCell(string puzzle, int index);

        void SetCellProbabilities(ref Cells cells, int index);

        Cells RunSolution(Cells board);  
    }
}
