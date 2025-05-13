using WebSudoku_v0._0._7.Data;
using WebSudoku_v0._0._7.Models;

namespace WebSudoku_v0._0._7.Repositories
{
    public class SudokuPuzzlesRepository(ApplicationDbContext _appDbContext) : ISudokuRepository
    {
        public List<DtoSudokuPuzzle> GetAllPuzzles()
        {
            var puzzles = _appDbContext.Puzzle.ToList();
            var model = puzzles.Select(p => new DtoSudokuPuzzle
            {
                Id = p.Id,
                Difficulty = p.Difficulty,
                BoardValues = p.BoardValues,
            }).ToList();
            return model;
        }

        public List<DtoSudokuPuzzle> GetEmptyReturnModel()
        {
            return new List<DtoSudokuPuzzle>
                {
                    new DtoSudokuPuzzle
                    {
                        Id = Guid.Empty,
                        Difficulty = int.MinValue,
                        BoardValues = string.Empty
                    }
                };
        }
    }
}
