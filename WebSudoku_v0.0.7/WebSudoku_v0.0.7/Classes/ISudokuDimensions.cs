namespace WebSudoku_v0._0._7.Classes
{
    public interface ISudokuDimensions
    {
        int Size { get; set; }
        int BlockSize { get; set; }
        int RowSize { get; set; }
        int ColumnSize { get; set; }
    }
}
