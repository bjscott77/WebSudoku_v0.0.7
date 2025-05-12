namespace Web_Core_SudokuSolver_v0._0._1.Classes
{
    public interface ICellLocation
    {
        int Row { get; set; }
        int Column { get; set; }
        int Block { get; set; }
        int Index { get; set; }

    }
}
