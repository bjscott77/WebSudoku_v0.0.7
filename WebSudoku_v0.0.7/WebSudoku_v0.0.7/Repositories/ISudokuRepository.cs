using WebSudoku_v0._0._7.Models;

namespace WebSudoku_v0._0._7.Repositories
{
    public interface ISudokuRepository
    {
        public Task<List<DTOSudoku>>? GetAllPuzzlesAsync();
        public Task<List<DTOSudoku>>? GetPuzzleAsync(string puzzle, string id);
        public Task<List<DTOSudoku>>? GetSolvedPuzzleAsync(string puzzle);
        public Task<List<DTOSudoku>>? AddPuzzleAsync(DTOSudoku puzzle);
        public Task<List<DTOSudoku>>? UpdatePuzzleAsync(List<DTOSudoku> puzzles);  
        public Task<List<DTOSudoku>>? DeletePuzzleAsync(string puzzle);     
        public List<DTOSudoku> GetEmptyListReturnModel();
        public DTOSudoku GetEmptyReturnModel();
    }
}
