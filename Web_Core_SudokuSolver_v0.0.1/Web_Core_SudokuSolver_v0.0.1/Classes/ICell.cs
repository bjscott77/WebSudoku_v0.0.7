namespace Web_Core_SudokuSolver_v0._0._1.Classes
{
    public interface ICell
    {
        public string DisplayValue { get; set; }
        public int Value { get; set; }
        public ICellLocation Location { get; set; }
        public bool isEnabled { get; set; }
        public bool hasValue { get; set; }
    }
}
