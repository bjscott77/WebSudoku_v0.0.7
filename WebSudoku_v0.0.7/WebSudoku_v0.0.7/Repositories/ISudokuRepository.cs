using WebSudoku_v0._0._7.Models;

namespace WebSudoku_v0._0._7.Repositories
{
    public interface ISudokuRepository
    {
        public List<DtoSudokuPuzzle> GetAllPuzzles();

        public List<DtoSudokuPuzzle> GetEmptyReturnModel();
    }
}
