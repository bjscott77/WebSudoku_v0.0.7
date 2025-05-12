namespace Web_Core_SudokuSolver_v0._0._1.Classes
{
    public class SudokuDimensions : ISudokuDimensions
    {
        public int Size { get; set; }
        public int BlockSize { get; set; }
        public int RowSize { get; set; }
        public int ColumnSize { get; set; }
        public SudokuDimensions(int size, int blockSize)
        {
            Size = size;
            int dims = (int)Math.Sqrt(size);
            BlockSize = dims;
            RowSize = dims;
            ColumnSize = dims;
        }
    }
    {
    }
}
