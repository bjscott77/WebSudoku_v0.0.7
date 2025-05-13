using WebSudoku_v0._0._7.Models;

namespace WebSudoku_v0._0._7.Repositories
{
    public interface ISudokuRepository
    {
        public List<DtoSudokuPuzzle> GetAllPuzzles();
        public List<DtoSudokuPuzzle> GetSelectedPuzzle();
        public List<DtoSudokuPuzzle> AddPuzzle(DtoSudokuPuzzle puzzle);
        public List<DtoSudokuPuzzle> UpdatePuzzle(DtoSudokuPuzzle puzzle);
        public List<DtoSudokuPuzzle> DeletePuzzle(DtoSudokuPuzzle puzzle);
        public List<DtoSudokuPuzzle> GetEmptyListReturnModel();
        public DtoSudokuPuzzle GetEmptyReturnModel();
    }
}
