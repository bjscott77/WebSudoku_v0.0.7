namespace WebSudoku_v0._0._7.Classes
{
    public class CellValueIndex
    {
        public char Value { get; set; }
        public int Index { get; set; }  
    }

    public class CellValueIndexPayload
    {
        public IEnumerable<CellValueIndex> CellsValueIndex { get; set; } = new List<CellValueIndex>();    
    }
}
