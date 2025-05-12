using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSudoku_v0._0._6.Data;
using WebSudoku_v0._0._6.Models;

namespace WebSudoku_v0._0._6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PuzzlesController(AppDbContext _context) : ControllerBase
    {
        [HttpGet]
        [Route("GetAllPuzzles")]
        public JsonResult GetAllPuzzles()
        {
            var puzzles = _context.Puzzles
                .Select(p => p)
                .ToList();

            if (puzzles == null || !puzzles.Any())
            {

                var dto404 = new SudokuPuzzleDTO(   
                    [string.Empty],
                    Guid.Empty,
                    string.Empty,
                    "No Puzzles found.", 404);

                List<SudokuPuzzleDTO> dtoList404 = new List<SudokuPuzzleDTO> { dto404 };
                dtoList404.Add(dto404);
                return new JsonResult(dtoList404);
            }

            List<SudokuPuzzleDTO> dtoList200 = new List<SudokuPuzzleDTO>();
            foreach (var puzzle in puzzles)
            {
                dtoList200.Add(new SudokuPuzzleDTO(
                [puzzle.Values],
                puzzle.Id,
                puzzle.Difficulty,
                "Puzzles retrieved successfully.", 200));
            }

            if (dtoList200 == null || !dtoList200.Any())
            {
                var dto404 = new SudokuPuzzleDTO(
                    [string.Empty],
                    Guid.Empty,
                    string.Empty,
                    "No Puzzles found.", 404);

                var dtoList404 = new List<SudokuPuzzleDTO>();
                dtoList404.Add(dto404);
                return new JsonResult(dtoList404);
            }

            return new JsonResult(dtoList200);
        }

        [HttpGet]
        [Route("GetPuzzleById/{id}")]
        public JsonResult GetPuzzleById(Guid id)
        {
            var puzzle = _context.Puzzles
                .Where(p => p.Id == id)
                .FirstOrDefault();

            if (puzzle == null)
            {
                var dto404 = new SudokuPuzzleDTO(
                    [string.Empty],
                    Guid.Empty,
                    string.Empty,
                    "Puzzle not found.", 404)
                {
                    Values = [string.Empty],
                    Id = Guid.Empty,
                    Difficulty = string.Empty,
                    Status = "Puzzle not found.",
                    StatusCode = 404
                };
                return new JsonResult(dto404);
            }
            
            var dto200 = new SudokuPuzzleDTO(
                [string.Empty],
                Guid.Empty,
                string.Empty,
                "Puzzle retrieved successfully.", 200)
            {
                Values = [puzzle.Values],
                Id = puzzle.Id,
                Difficulty = puzzle.Difficulty,
                Status = "Puzzle retrieved successfully.",
                StatusCode = 200
            };
            
            return new JsonResult(dto200);
        }

        [HttpPost]
        [Route("AddPuzzle")]
        public JsonResult AddPuzzle([FromBody] SudokuPuzzleEntityModel puzzle)
        {
            if (puzzle == null)
            {
                var dto400 = new SudokuPuzzleDTO(
                    [string.Empty],
                    Guid.Empty,
                    string.Empty,
                    "Invalid puzzle data.", 400);

                return new JsonResult(dto400);
            }

            _context.Add(puzzle);
            _context.SaveChanges();

            var dto200 = new SudokuPuzzleDTO(
                [puzzle.Values],
                puzzle.Id,
                puzzle.Difficulty,
                "Puzzle added successfully.", 200);

            return new JsonResult(puzzle);
        }

        [HttpPut]
        [Route("UpdatePuzzle/{id}")]
        public JsonResult UpdatePuzzle(Guid id, [FromBody] SudokuPuzzleEntityModel puzzle)
        {
            if (puzzle == null || id != puzzle.Id)
            {
                var dto400 = new SudokuPuzzleDTO(
                    [string.Empty],
                    Guid.Empty,
                    string.Empty,
                    "Invalid puzzle data.", 400)
                {
                    Values = [string.Empty],
                    Id = Guid.Empty,
                    Difficulty = string.Empty,
                    Status = "Invalid puzzle data.",
                    StatusCode = 400
                };
                return new JsonResult(dto400);
            }
            
            var existingPuzzle = _context.Puzzles
                .Where(p => p.Id == id)
                .FirstOrDefault();

            if (existingPuzzle == null)
            {
                var dto404 = new SudokuPuzzleDTO(
                    [string.Empty],
                    Guid.Empty,
                    string.Empty,
                    "Puzzle not found.", 404);

                return new JsonResult(dto404);
            }
            
            _context.Puzzles.Update(puzzle);
            _context.SaveChanges();

            var dto204 = new SudokuPuzzleDTO(
                [string.Empty],
                Guid.Empty,
                string.Empty,
                "Puzzle updated successfully.", 200)
            {
                Values = [string.Empty],
                Id = Guid.Empty,
                Difficulty = string.Empty,
                Status = "Puzzle updated successfully.",
                StatusCode = 200
            };
            return new JsonResult(dto204);
        }

        [HttpDelete]
        [Route("DeletePuzzle/{id}")]
        public JsonResult DeletePuzzle(Guid id)
        {
            var existingPuzzle = _context.Puzzles
                .Where(p => p.Id == id)
                .FirstOrDefault();

            if (existingPuzzle == null)
            {
                var dto404 = new SudokuPuzzleDTO(
                    [string.Empty],
                    Guid.Empty,
                    string.Empty,
                    "Puzzle not found.", 404)
                {
                    Values = [string.Empty],
                    Id = Guid.Empty,
                    Difficulty = string.Empty,
                    Status = "Puzzle not found.",
                    StatusCode = 404
                };
                return new JsonResult(dto404);
            }
            
            var numOfDeletes = _context.Puzzles.
                Where(p => p.Id == id)
                .ExecuteDelete();

            if (numOfDeletes == 0)
            {
                var dto404 = new SudokuPuzzleDTO(
                    [string.Empty],
                    Guid.Empty,
                    string.Empty,
                    "Puzzle not found.", 404);

                return new JsonResult(dto404);
            }

            var dto204 = new SudokuPuzzleDTO(
                [string.Empty],
                Guid.Empty,
                string.Empty,
                "Puzzle deleted successfully.", 204);

            return new JsonResult(dto204);
        }
    }
}
