namespace WebSudoku_v0._0._7.Classes
{
    public interface ICell
    {
        string DisplayValue { get; set; }
        int Value { get; set; }
        ICellLocation Location { get; set; }
        ICellPossibilities CellPossibilities { get; set; }
        bool isEnabled { get; set; }
        bool hasValue { get; set; }
    }
}
