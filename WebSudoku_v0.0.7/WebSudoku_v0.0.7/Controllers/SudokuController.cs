using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebSudoku_v0._0._7.Data;

namespace WebSudoku_v0._0._7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SudokuController(ApplicationDbContext _appDbContext) : ControllerBase
    {
        [HttpGet("GetAllPuzzles")]
        [Route("/getallpuzzles")]
        public JsonResult Get()
        {
            var puzzles = _appDbContext.Puzzle.ToList();

            var model = JsonSerializer.Serialize(puzzles);
            return new JsonResult(model);
        }    
    }
}
