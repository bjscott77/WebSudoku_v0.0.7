using WebSudoku_v0._0._7.Models;

namespace WebSudoku_v0._0._7.Repositories
{
    public interface ISudokuRepository
    {
        public Task<List<SudokuPuzzledto>>? GetAllPuzzlesAsync();
        public Task<List<SudokuPuzzledto>>? GetPuzzleAsync(string puzzle);
        public Task<List<SudokuPuzzledto>>? GetSolvedPuzzleAsync(string puzzle);
        public Task<List<SudokuPuzzledto>>? AddPuzzleAsync(SudokuPuzzledto puzzle);
        public Task<List<SudokuPuzzledto>>? UpdatePuzzleAsync(List<SudokuPuzzledto> puzzles);  
        public Task<List<SudokuPuzzledto>>? DeletePuzzleAsync(string puzzle);     
        public List<SudokuPuzzledto> GetEmptyListReturnModel();
        public SudokuPuzzledto GetEmptyReturnModel();
    }
}
