using WebSudoku_v0._0._7.Models;

namespace WebSudoku_v0._0._7.Repositories
{
    public interface ISudokuRepository
    {
        public List<SudokuPuzzledto> GetAllPuzzles();
        public List<SudokuPuzzledto> GetSelectedPuzzle();
        public List<SudokuPuzzledto> AddPuzzle(SudokuPuzzledto puzzle);
        public List<SudokuPuzzledto> UpdatePuzzle(SudokuPuzzledto puzzle);
        public List<SudokuPuzzledto> DeletePuzzle(SudokuPuzzledto puzzle);
        public List<SudokuPuzzledto> GetEmptyListReturnModel();
        public SudokuPuzzledto GetEmptyReturnModel();
    }
}
