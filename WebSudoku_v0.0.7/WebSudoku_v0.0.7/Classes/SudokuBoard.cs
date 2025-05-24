namespace WebSudoku_v0._0._7.Classes
{
    public class SudokuBoard(IConfigurationSection _devConfig) : ISudokuBoard   
    {
        public ISudokuManager SudokuManager { get; set; } = new SudokuManager();
        public Cells Cells { get; set; } = new Cells();
        public DevConfiguration DevConfiguration { get; set; } = new DevConfiguration(_devConfig);
        public required ISudokuDimensions Dimensions { get; set; }

        public SudokuBoard() : this(null)
        {
        }

        public void DisplayOdds()
        {
            foreach(var cell in Cells.List)
            {
                var odds = string.Join(", ", cell.CellPossibilities.List);
                Console.WriteLine(odds);
            }
        }
        
        public void InitializeOdds()
        {
            for (int i = 0; i < Cells.List.Count; i++)
            {
                Cells = SudokuManager.InitialOddsSetup(Cells, i);
                Cells = SudokuManager.SetCellOdds(Cells, i);
            }
        }

        public void InitializeBoard(string puzzle)
        {

            var dims = DevConfiguration.SudokuSettings.BoardDimensions;
            Dimensions = new SudokuDimensions(dims.FirstOrDefault(), dims.LastOrDefault());
            Cells = new Cells();
            int index = 0;
            foreach (var cellValue in puzzle.ToCharArray())
            {
                Cells.List.Add(SudokuManager.SetNextCell(cellValue.ToString(), index));
                index++;
            }
        }
    }
}
