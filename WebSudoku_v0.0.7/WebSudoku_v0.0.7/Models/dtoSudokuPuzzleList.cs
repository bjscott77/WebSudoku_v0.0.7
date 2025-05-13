namespace WebSudoku_v0._0._7.Models
{
    public class DtoSudokuPuzzleList
    {
        public DtoSudokuPuzzle[] Puzzles { get; set; } = new DtoSudokuPuzzle[] { };
        public DtoSudokuPuzzleList()
        {
        }

        public DtoSudokuPuzzleList(DtoSudokuPuzzle[] collection)
        {
            Puzzles.ToList().AddRange(collection);            
        }
    }
}
