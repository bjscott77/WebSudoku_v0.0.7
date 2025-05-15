using System;

namespace WebSudoku_v0._0._7.Classes
{
    public class SudokuDimensions : ISudokuDimensions
    {
        public int Size { get; set; }
        public int BlockSize { get; set; }
        public int RowSize { get; set; }
        public int ColumnSize { get; set; }

        public SudokuDimensions() 
        {
            Size = 81;
            BlockSize = 9;
            RowSize = 9;
            ColumnSize = 9;
        }
        public SudokuDimensions(int size)
        {
            Size = size;
            BlockSize = (int)Math.Sqrt(size);
            RowSize = BlockSize;
            ColumnSize = BlockSize;
        }

        public SudokuDimensions(int size, int blockSize)
        {
            Size = size;
            BlockSize = blockSize;
            RowSize = blockSize;
            ColumnSize = blockSize;
        }
    }
}
