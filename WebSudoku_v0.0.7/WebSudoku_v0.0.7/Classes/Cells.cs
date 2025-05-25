namespace WebSudoku_v0._0._7.Classes
{
    public class Cells
    {
        public List<Cell> List { get; set; } = new List<Cell>();
        public Cells() { }

        public List<Cell> CopyCells(List<Cell> cells)
        {
            List.Clear();
            foreach (var cell in cells)
            {
                var c = cell;
                this.List.Add(c);
            }
            return List;
        }
    }
}
