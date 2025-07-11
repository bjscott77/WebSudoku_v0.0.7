using Microsoft.EntityFrameworkCore;
using WebSudoku_v0._0._7.Classes;
using WebSudoku_v0._0._7.Configuration;
using WebSudoku_v0._0._7.Data;
using WebSudoku_v0._0._7.Models;

namespace WebSudoku_v0._0._7.Repositories
{
    public class SudokuRepository : ISudokuRepository
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly ISudokuBoard _sudokuBoard;
        private readonly DevConfiguration _devConfig;
        private readonly ILogger<SudokuRepository> _debugLogger;
        private readonly SudokuAPIHelpers _helpers;
        public SudokuRepository(ApplicationDbContext appDbContext, ISudokuBoard sudokuBoard, DevConfiguration devConfig, ILogger<SudokuRepository> logger, SudokuAPIHelpers helpers)
        {
            _appDbContext = appDbContext;
            _sudokuBoard = sudokuBoard;
            _devConfig = devConfig;
            _debugLogger = logger;
            _helpers = helpers;
        }
        public async Task<List<SudokuDTO>>? AddPuzzleAsync(SudokuDTO? puzzle)
        {
            try
            {
                if (puzzle == null || _appDbContext == null)
                    return await Task.FromResult<List<SudokuDTO>>(null);

                var existingPuzzle = await _appDbContext.Puzzle.FirstOrDefaultAsync(p => p.BoardValues == puzzle.BoardValues);
                if (existingPuzzle != null)
                    return await Task.FromResult<List<SudokuDTO>>(null);

                await _appDbContext.Puzzle.AddAsync(puzzle);
            }
            catch (Exception ex)
            {
                _debugLogger.LogError($"Error in SudokuPuzzleRepository AddPuzzle(...).  Error: {ex.Message}.  InnerMessage: {ex.InnerException?.Message}.");
                return await Task.FromResult<List<SudokuDTO>>(null);
            }

            await _appDbContext.SaveChangesAsync();

            List<SudokuDTO> added;

            if (_devConfig.SudokuSettings.GamePlaySettings.SolveSettings.Selection == "NEW")
            {
                added = new List<SudokuDTO>() { puzzle, };
                added.AddRange(_appDbContext.Puzzle.Where(p => !(p.BoardValues == puzzle.BoardValues)).ToList());
                return added;
            }

            return await GetAllPuzzlesAsync();
        }

        public async Task<List<SudokuDTO>>? DeletePuzzleAsync(string puzzle)
        {
            try
            {
                if (_appDbContext == null || string.IsNullOrEmpty(puzzle))
                    return await Task.FromResult<List<SudokuDTO>>(null);

                var existingPuzzle = await _appDbContext.Puzzle.FirstOrDefaultAsync(p => p.BoardValues == puzzle);
                if (existingPuzzle == null)
                    return await Task.FromResult<List<SudokuDTO>>(null);

                _appDbContext.Puzzle.Remove(existingPuzzle);
            }
            catch (Exception ex)
            {
                _debugLogger.LogError($"Error in SudokuPuzzleRepository DeletePuzzle(...).  Error: {ex.Message}.  InnerMessage: {ex.InnerException?.Message}.");
                return await Task.FromResult<List<SudokuDTO>>(null);
            }

            await _appDbContext.SaveChangesAsync();
            return await GetAllPuzzlesAsync();
        }

        public async Task<List<SudokuDTO>>? GetPuzzleAsync(string puzzle, string id)
        {
            if (string.IsNullOrEmpty(puzzle) || _appDbContext == null)
                return await Task.FromResult<List<SudokuDTO>>(null);

            try
            {
                _sudokuBoard.createSudokuBoard(puzzle);
                _sudokuBoard.InitializeProbabilities();

                var possibles = _sudokuBoard.GetCells().List.Select(c => string.Join(",", c.CellPossibilities.List.Where(p => p > 0)));

                return await _appDbContext.Puzzle
                    .Where(p => p.Id == Guid.Parse(id))
                    .Select(record => new SudokuDTO
                    {
                        Id = record.Id,
                        Difficulty = record.Difficulty,
                        BoardValues = puzzle,
                        Possibles = possibles
                    })
                        .ToListAsync();
            }
            catch (Exception ex)
            {
                _debugLogger.LogError($"GetPuzzleAsync(...).  Error: {ex.Message}.  InnerMessage: {ex.InnerException?.Message}.");
            }
            return GetEmptyListReturnModel();
        }

        public async Task<List<SudokuDTO>>? GetAllPuzzlesAsync()
        {
            if (_appDbContext == null)
                return await new Task<List<SudokuDTO>>(null);

            try
            {
                var result = await _appDbContext.Puzzle
                    .Where(p => p != null)
                    .Select(p => new SudokuDTO
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
            catch (Exception ex)
            {
                _debugLogger.LogError($"GetAllPuzzlesAsync(...).  Error: {ex.Message}.  InnerMessage: {ex.InnerException?.Message}.");
            }
            return GetEmptyListReturnModel();
        }

        public async Task<List<SudokuDTO>> GetSolvedPuzzleAsync(string puzzle)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(puzzle) || _sudokuBoard == null)
                    return null;

                try
                {
                    _sudokuBoard.createSudokuBoard(puzzle);
                    _sudokuBoard.InitializeProbabilities();
                    _sudokuBoard.SudokuManager.RunSolution(_sudokuBoard.GetCells());

                    if (_sudokuBoard.GetCells().List == null || _sudokuBoard.GetCells().List.Count == 0)
                        return null;

                    return new List<SudokuDTO>
                {
                    new SudokuDTO
                    {
                        Id = Guid.Empty,
                        Difficulty = 0,
                        BoardValues = string.Join("", _sudokuBoard.GetCells().List.Select(c => c.Value))
                    }
                };
                }
                catch (Exception ex)
                {
                    _debugLogger.LogError($"GetSolvedPuzzleAsync(...).  Error: {ex.Message}.  InnerMessage: {ex.InnerException?.Message}.");
                }
                return GetEmptyListReturnModel();
            });
        }

        public async Task<List<SudokuDTO>>? UpdatePuzzleAsync(List<SudokuDTO> puzzles)
        {
            try
            {
                if (_appDbContext == null || puzzles == null)
                    return await Task.FromResult<List<SudokuDTO>>(null);

                var existingPuzzle = await _appDbContext.Puzzle.FirstOrDefaultAsync(p => p.BoardValues == puzzles[0].BoardValues);
                if (existingPuzzle == null)
                    return await Task.FromResult<List<SudokuDTO>>(null);

                existingPuzzle.BoardValues = puzzles[1].BoardValues;
                existingPuzzle.Difficulty = puzzles[1].Difficulty;
                _appDbContext.Puzzle.Update(existingPuzzle);
            }
            catch (Exception ex)
            {
                _debugLogger.LogError($"UpdatePuzzleAsync(...).  Error: {ex.Message}.  InnerMessage: {ex.InnerException?.Message}.");
                return await Task.FromResult<List<SudokuDTO>>(null);
            }
            if (_appDbContext == null)
                return await Task.FromResult<List<SudokuDTO>>(null);

            await _appDbContext.SaveChangesAsync();

            var retObj = new List<SudokuDTO>()
            {
                new SudokuDTO()
                {
                    BoardValues = puzzles[1].BoardValues
                }
            };
            var retPuzzles = await GetAllPuzzlesAsync();
            retObj.AddRange(retPuzzles);
            return retObj;
        }

        public List<SudokuDTO> GetEmptyListReturnModel()
        {
            return new List<SudokuDTO>
                {
                    new SudokuDTO
                    {
                        Id = Guid.Empty,
                        Difficulty = int.MinValue,
                        BoardValues = string.Empty
                    }
                };
        }

        public SudokuDTO GetEmptyReturnModel()
        {
            return new SudokuDTO
            {
                Id = Guid.Empty,
                Difficulty = int.MinValue,
                BoardValues = string.Empty
            };
        }

        public SudokuResponse UpdateSettings(SudokuResponse response, string puzzle)
        {            
            return _helpers.AttachSettings(response, puzzle);
        }
    }
}
