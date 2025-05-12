namespace WebSudoku_v0._0._7.Models
{
    public class DtoSudokuPuzzleList
    {
        public List<IDtoSudokuPuzzle> Puzzles { get; set; } = new List<IDtoSudokuPuzzle>();

        public DtoSudokuPuzzleList()
        {
        }

        public DtoSudokuPuzzleList(IEnumerable<IDtoSudokuPuzzle> collection)
        {
            Puzzles.AddRange(collection);            
        }
    }
}
