using WebSudoku_v0._0._7.Data;
using WebSudoku_v0._0._7.Models;

namespace WebSudoku_v0._0._7.Repositories
{
    public class SudokuPuzzlesRepository(ApplicationDbContext _appDbContext) : ISudokuRepository
    {
        public List<SudokuPuzzledto> AddPuzzle(SudokuPuzzledto puzzle)
        {
            var testList = _appDbContext.Puzzle.ToList();   
            if (testList != null && testList.Count > 0)
            {
                var test = testList.Where(t => t.BoardValues == puzzle.BoardValues);
                if (test != null)
                    return null;
            }

            _appDbContext.Puzzle.Add(puzzle);
            _appDbContext.SaveChanges();
            return GetAllPuzzles();
        }

        public List<SudokuPuzzledto> DeletePuzzle(SudokuPuzzledto puzzle)
        {
            var test = _appDbContext.Puzzle.Find(puzzle.Id);
            if (test == null)
            {
                return null;
            }

            _appDbContext.Puzzle.Remove(puzzle);
            _appDbContext.SaveChanges();
            return GetAllPuzzles();
        }

        public List<SudokuPuzzledto> GetAllPuzzles()
        {
            var puzzles = _appDbContext.Puzzle.ToList();
            var model = puzzles.Select(p => new SudokuPuzzledto
            {
                Id = p.Id,
                Difficulty = p.Difficulty,
                BoardValues = p.BoardValues,
            }).ToList();
            return model;
        }

        public List<SudokuPuzzledto> GetEmptyListReturnModel()
        {
            return new List<SudokuPuzzledto>
                {
                    new SudokuPuzzledto
                    {
                        Id = Guid.Empty,
                        Difficulty = int.MinValue,
                        BoardValues = string.Empty
                    }
                };
        }

        public SudokuPuzzledto GetEmptyReturnModel()
        {
            return new SudokuPuzzledto
            {
                Id = Guid.Empty,
                Difficulty = int.MinValue,
                BoardValues = string.Empty
            };
        }

        public List<SudokuPuzzledto> GetSelectedPuzzle()
        {
            //Ignored
            return new List<SudokuPuzzledto>
            {
                new SudokuPuzzledto
                {
                    Id = Guid.Empty,
                    Difficulty = int.MinValue,
                    BoardValues = string.Empty
                }
            };
        }

        public List<SudokuPuzzledto> UpdatePuzzle(SudokuPuzzledto puzzle)
        {
            throw new NotImplementedException();
        }
    }
}
