using Microsoft.EntityFrameworkCore;
using WebSudoku_v0._0._7.Classes;
using WebSudoku_v0._0._7.Data;
using WebSudoku_v0._0._7.Models;

namespace WebSudoku_v0._0._7.Repositories
{
    public class SudokuPuzzlesRepository(ApplicationDbContext _appDbContext, ISudokuBoard _sudokuBoard, DevConfiguration _devConfig) : ISudokuRepository
    {
        public async Task<List<SudokuPuzzledto>>? AddPuzzleAsync(SudokuPuzzledto? puzzle)
        {
            try
            {
                if (puzzle == null || _appDbContext == null)
                    return await Task.FromResult<List<SudokuPuzzledto>>(null);

                var existingPuzzle = await _appDbContext.Puzzle.FirstOrDefaultAsync(p => p.BoardValues == puzzle.BoardValues);
                if (existingPuzzle != null)
                    return await Task.FromResult<List<SudokuPuzzledto>>(null);

                await _appDbContext.Puzzle.AddAsync(puzzle);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SudokuPuzzleRepository AddPuzzle(...).  Error: {ex.Message}.  InnerMessage: {ex.InnerException?.Message}.");
                return await Task.FromResult<List<SudokuPuzzledto>>(null);
            }

            await _appDbContext.SaveChangesAsync();

            List<SudokuPuzzledto> added;

            if (_devConfig.SudokuSettings.GamePlaySettings.SolveSettings.Selection == "NEW")
            {
                added = new List<SudokuPuzzledto>() { puzzle, };
                added.AddRange(_appDbContext.Puzzle.Where(p => !(p.BoardValues == puzzle.BoardValues)).ToList());
                return added;
            }

            return await GetAllPuzzlesAsync();
        }

        public async Task<List<SudokuPuzzledto>>? DeletePuzzleAsync(string puzzle)
        {
            try
            {
                if (_appDbContext == null || string.IsNullOrEmpty(puzzle))
                    return await Task.FromResult<List<SudokuPuzzledto>>(null);

                var existingPuzzle = await _appDbContext.Puzzle.FirstOrDefaultAsync(p => p.BoardValues == puzzle);
                if (existingPuzzle == null)
                    return await Task.FromResult<List<SudokuPuzzledto>>(null);

                _appDbContext.Puzzle.Remove(existingPuzzle);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SudokuPuzzleRepository DeletePuzzle(...).  Error: {ex.Message}.  InnerMessage: {ex.InnerException?.Message}.");
                return await Task.FromResult<List<SudokuPuzzledto>>(null);
            }

            await _appDbContext.SaveChangesAsync();
            return await GetAllPuzzlesAsync();
        }

        public async Task<List<SudokuPuzzledto>>? GetPuzzleAsync(string puzzle, string id)
        {
            if (string.IsNullOrEmpty(puzzle) || _appDbContext == null)
                return await Task.FromResult<List<SudokuPuzzledto>>(null);

            _sudokuBoard.createSudokuBoard(puzzle);
            _sudokuBoard.InitializeProbabilities();

            var possibles = _sudokuBoard.GetCells().List.Select(c => string.Join(",", c.CellPossibilities.List.Where(p => p > 0)));

            return await _appDbContext.Puzzle
                .Where(p => p.Id == Guid.Parse(id))
                .Select(record => new SudokuPuzzledto
                {
                    Id = record.Id,
                    Difficulty = record.Difficulty,
                    BoardValues = puzzle,
                    Possibles = possibles
                })
                    .ToListAsync();
        }

        public async Task<List<SudokuPuzzledto>>? GetAllPuzzlesAsync()
        {
            if (_appDbContext == null)
                return await new Task<List<SudokuPuzzledto>>(null);

            var result = await _appDbContext.Puzzle
                .Where(p => p != null)
                .Select(p => new SudokuPuzzledto
                {
                    Id = p.Id,
                    Difficulty = p.Difficulty,
                    BoardValues = p.BoardValues,
                }).ToListAsync();

            _sudokuBoard.createSudokuBoard(result[0].BoardValues);
            _sudokuBoard.InitializeProbabilities();

            result[0].Possibles = _sudokuBoard.GetCells().List.Select(c => string.Join(",", c.CellPossibilities.List.Where(p => p > 0)));

            return result;
        }

        public async Task<List<SudokuPuzzledto>> GetSolvedPuzzleAsync(string puzzle)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(puzzle) || _sudokuBoard == null)
                    return null;

                _sudokuBoard.createSudokuBoard(puzzle);
                _sudokuBoard.InitializeProbabilities();
                _sudokuBoard.SudokuManager.RunSolution(_sudokuBoard.GetCells());

                if (_sudokuBoard.GetCells().List == null || _sudokuBoard.GetCells().List.Count == 0)
                    return null;

                return new List<SudokuPuzzledto>
                {
                    new SudokuPuzzledto
                    {
                        Id = Guid.Empty,
                        Difficulty = 0,
                        BoardValues = string.Join("", _sudokuBoard.GetCells().List.Select(c => c.Value))
                    }
                };
            });
        }

        public async Task<List<SudokuPuzzledto>>? UpdatePuzzleAsync(List<SudokuPuzzledto> puzzles)
        {
            try
            {
                if (_appDbContext == null || puzzles == null)
                    return await Task.FromResult<List<SudokuPuzzledto>>(null);

                var existingPuzzle = await _appDbContext.Puzzle.FirstOrDefaultAsync(p => p.BoardValues == puzzles[0].BoardValues);
                if (existingPuzzle == null)
                    return await Task.FromResult<List<SudokuPuzzledto>>(null);

                existingPuzzle.BoardValues = puzzles[1].BoardValues;
                existingPuzzle.Difficulty = puzzles[1].Difficulty;
                _appDbContext.Puzzle.Update(existingPuzzle);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SudokuPuzzleRepository UpdatePuzzle(...).  Error: {ex.Message}.  InnerMessage: {ex.InnerException?.Message}.");
                return await Task.FromResult<List<SudokuPuzzledto>>(null);
            }
            if (_appDbContext == null)
                return await Task.FromResult<List<SudokuPuzzledto>>(null);

            await _appDbContext.SaveChangesAsync();

            var retObj = new List<SudokuPuzzledto>()
            {
                new SudokuPuzzledto()
                {
                    BoardValues = puzzles[1].BoardValues
                }
            };
            var retPuzzles = await GetAllPuzzlesAsync();
            retObj.AddRange(retPuzzles);
            return retObj;
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
    }
}
