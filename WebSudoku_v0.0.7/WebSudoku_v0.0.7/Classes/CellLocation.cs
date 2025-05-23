﻿namespace WebSudoku_v0._0._7.Classes
{
    public class CellLocation : ICellLocation
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Block { get; set; }
        public int Index { get; set; }

        public CellLocation(int row, int column, int block, int index)
        {
            Row = row;
            Column = column;
            Block = block;
            Index = index;
        }
    }
}
