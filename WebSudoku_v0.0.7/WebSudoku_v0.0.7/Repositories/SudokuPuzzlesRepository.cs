using WebSudoku_v0._0._7.Data;
using WebSudoku_v0._0._7.Models;

namespace WebSudoku_v0._0._7.Repositories
{
    public class SudokuPuzzlesRepository(ApplicationDbContext _appDbContext) : ISudokuRepository
    {
        public List<DtoSudokuPuzzle> AddPuzzle(DtoSudokuPuzzle puzzle)
        {
            var newPuzzle = new DtoSudokuPuzzle
            {
                Id = Guid.NewGuid(),
                Difficulty = puzzle.Difficulty,
                BoardValues = puzzle.BoardValues
            };
            _appDbContext.Puzzle.Add(newPuzzle);
            _appDbContext.SaveChanges();
            return GetAllPuzzles();
        }

        public List<DtoSudokuPuzzle> DeletePuzzle(DtoSudokuPuzzle puzzle)
        {
            throw new NotImplementedException();
        }

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

        public List<DtoSudokuPuzzle> GetEmptyListReturnModel()
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

        public DtoSudokuPuzzle GetEmptyReturnModel()
        {
            throw new NotImplementedException();
        }

        public List<DtoSudokuPuzzle> GetSelectedPuzzle()
        {
            throw new NotImplementedException();
        }

        public List<DtoSudokuPuzzle> UpdatePuzzle(DtoSudokuPuzzle puzzle)
        {
            throw new NotImplementedException();
        }
    }
}
