
namespace Web_Core_SudokuSolver_v0._0._1.Classes
{
    public class SudokuBoard : ISudokuBoard
    {
        public ISudokuManager SudokuManager { get; set; }
        public Cells Cells { get; set; }
        public ISudokuDimensions Dimensions { get; set; }
        public string CurrentStartingPuzzle { get; set; } = string.Empty;

        public SudokuBoard(SudokuDimensions dimensions)
        {
            SudokuManager = new SudokuManager(dimensions);
            Cells = new Cells();
            Dimensions = dimensions;

            this.InitializeBoard();
        }

        private bool InitializeBoard()
        {
            Console.WriteLine("Paste the puzzle here:");
            this.CurrentStartingPuzzle = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrEmpty(this.CurrentStartingPuzzle))
            {
                Console.WriteLine("Invalid puzzle input.");
                return false;
            }

            int index = 0;
            foreach (string cellValue in this.CurrentStartingPuzzle.Split(","))
            {
                Cells.List.Add(SudokuManager.SetNextCell(cellValue, index));
                index++;
            }

            Cells.List.ForEach(cell =>
            {
                Console.WriteLine($"Cell: {cell.Location.Row}, {cell.Location.Column}, {cell.Location.Block}, {cell.Location.Index} - Value: {cell.DisplayValue}");
            });
            return true;
        }
    }
}
