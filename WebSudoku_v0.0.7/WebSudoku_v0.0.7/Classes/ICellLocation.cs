namespace WebSudoku_v0._0._7.Classes
{
    public interface ICellLocation
    {
        int Row { get; set; }
        int Column { get; set; }
        int Block { get; set; }
        int Index { get; set; }

    }
}
