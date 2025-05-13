using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebSudoku_v0._0._7.Data;
using WebSudoku_v0._0._7.Models;
using WebSudoku_v0._0._7.Repositories;

namespace WebSudoku_v0._0._7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SudokuController(ISudokuRepository _sudokuRepo) : ControllerBase
    {
        [HttpGet("GetAllPuzzles")]
        [Route("/getallpuzzles")]
        public JsonResult Get()
        {
            try
            {
                var model = _sudokuRepo.GetAllPuzzles();
                var json = JsonSerializer.Serialize(model);
                return new JsonResult(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SudokuController GET.  Error Message: {ex.Message}.  Inner Exception: {ex?.InnerException?.Message}");
                var model = _sudokuRepo.GetEmptyListReturnModel();

                var json = JsonSerializer.Serialize(model);
                return new JsonResult(json);
            }
        }

        [HttpPost("AddPuzzle")]
        [Route("/addpuzzle")]
        public async Task<JsonResult> AddPuzzle()
        {
            try
            {
                var json = new StreamReader(Request.Body).ReadToEndAsync();
                var puzzle = JsonSerializer.Deserialize<DtoSudokuPuzzle>(json.Result);
                var model = _sudokuRepo.AddPuzzle(puzzle);
                var serial = JsonSerializer.Serialize(model);
                return new JsonResult(serial);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SudokuController POST.  Error Message: {ex.Message}.  Inner Exception: {ex?.InnerException?.Message}");
                var model = _sudokuRepo.GetEmptyListReturnModel();
                var json = JsonSerializer.Serialize(model);
                return new JsonResult(json);
            }
        }
    }
}
