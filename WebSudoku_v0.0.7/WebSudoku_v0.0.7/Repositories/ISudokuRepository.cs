using WebSudoku_v0._0._7.Models;

namespace WebSudoku_v0._0._7.Repositories
{
    public interface ISudokuRepository
    {
        public Task<List<SudokuDTO>>? GetAllPuzzlesAsync();
        public Task<List<SudokuDTO>>? GetPuzzleAsync(string puzzle, string id);
        public Task<List<SudokuDTO>>? GetSolvedPuzzleAsync(string puzzle);
        public Task<List<SudokuDTO>>? AddPuzzleAsync(SudokuDTO puzzle);
        public Task<List<SudokuDTO>>? UpdatePuzzleAsync(List<SudokuDTO> puzzles);  
        public Task<List<SudokuDTO>>? DeletePuzzleAsync(string puzzle);     
        public List<SudokuDTO> GetEmptyListReturnModel();
        public SudokuDTO GetEmptyReturnModel();
        public SudokuResponse UpdateSettings(SudokuResponse response, string puzzle);
    }
}
