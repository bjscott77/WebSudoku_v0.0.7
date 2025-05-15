using WebSudoku_v0._0._7.Data;
using WebSudoku_v0._0._7.Models;

namespace WebSudoku_v0._0._7.Repositories
{
    public class SudokuPuzzlesRepository(ApplicationDbContext? _appDbContext) : ISudokuRepository
    {
        public SudokuPuzzlesRepository() : this(null)
        {
        }

        public List<SudokuPuzzledto>? AddPuzzle(SudokuPuzzledto puzzle)
        {
            try
            {
                var existingPuzzle = _appDbContext?.Puzzle.FirstOrDefault(p => p.BoardValues == puzzle.BoardValues);
                if (existingPuzzle != null)
                    return null;
                
                _appDbContext?.Puzzle.Add(puzzle);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SudokuPuzzleRepository AddPuzzle(...).  Error: {ex.Message}.  InnerMessage: {ex.InnerException?.Message}.");
                return null;
            }
            if (_appDbContext == null)
                return null;
            
            _appDbContext.SaveChanges();
            return GetAllPuzzles();
        }

        public List<SudokuPuzzledto>? DeletePuzzle(SudokuPuzzledto puzzle)
        {
            try
            {
                var existingPuzzle = _appDbContext?.Puzzle.FirstOrDefault(p => p.Id == puzzle.Id);
                if (existingPuzzle != null)
                    return null;

                _appDbContext?.Puzzle.Remove(puzzle);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SudokuPuzzleRepository AddPuzzle(...).  Error: {ex.Message}.  InnerMessage: {ex.InnerException?.Message}.");
                return null;
            }
            if (_appDbContext == null)
                return null;

            _appDbContext.SaveChanges();
            return GetAllPuzzles();
        }

        public List<SudokuPuzzledto> GetAllPuzzles()
        {
            var puzzles = _appDbContext?.Puzzle.ToList();
            var model = puzzles?.Select(p => new SudokuPuzzledto
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
            return null;
        }

        public List<SudokuPuzzledto> UpdatePuzzle(SudokuPuzzledto puzzle)
        {
            return null;
        }
    }
}
