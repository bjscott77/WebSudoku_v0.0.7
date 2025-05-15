namespace WebSudoku_v0._0._7.Classes
{
    public class Cell : ICell
    {
        public string DisplayValue { get; set; } = string.Empty;
        public int Value { get; set; }
        public ICellLocation Location { get; set; }
        public bool isEnabled { get; set; } = true;
        public bool hasValue { get; set; } = false;
        public ICellPossibilities CellPossibilities { get; set; } = new CellPossibilities();

        public Cell(CellLocation location)
        {
            Location = location;
        }
    }
}
