using WebSudoku_v0._0._7.Models;

namespace WebSudoku_v0._0._7.Repositories
{
    public interface ISudokuRepository
    {
        public List<SudokuPuzzledto>? GetAllPuzzles();
        public List<SudokuPuzzledto>? GetPuzzle(string puzzle);
        public List<SudokuPuzzledto>? GetSolvedPuzzle(string puzzle);
        public Task<List<SudokuPuzzledto>>? AddPuzzleAsync(SudokuPuzzledto puzzle);
        public Task<List<SudokuPuzzledto>>? UpdatePuzzleAsync(List<SudokuPuzzledto> puzzles);  
        public Task<List<SudokuPuzzledto>>? DeletePuzzleAsync(string puzzle);     
        public List<SudokuPuzzledto> GetEmptyListReturnModel();
        public SudokuPuzzledto GetEmptyReturnModel();
    }
}
