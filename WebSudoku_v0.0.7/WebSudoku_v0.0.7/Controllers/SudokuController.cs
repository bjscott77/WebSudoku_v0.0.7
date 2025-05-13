using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebSudoku_v0._0._7.Data;
using WebSudoku_v0._0._7.Models;
using WebSudoku_v0._0._7.Repositories;

namespace WebSudoku_v0._0._7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors()]
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
                var json = await new StreamReader(Request.Body).ReadToEndAsync();
                if (string.IsNullOrEmpty(json))
                {
                    var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                    var errorResponse = new AddPuzzleResponsedto(errorModel, 400, "Bad Request", "Request body is empty");
                    var jsonResult = JsonSerializer.Serialize(errorResponse);
                    return new JsonResult(jsonResult);
                }

                var puzzle = JsonSerializer.Deserialize<SudokuPuzzledto>(json);
                if (puzzle == null)
                {
                    var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                    var errorResponse = new AddPuzzleResponsedto(errorModel, 400, "Bad Request", "Request body is missing board values in the puzzle.");
                    var jsonResult = JsonSerializer.Serialize(errorResponse);
                    return new JsonResult(jsonResult);
                }
                if (string.IsNullOrEmpty(puzzle.BoardValues))
                {
                    var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                    var errorResponse = new AddPuzzleResponsedto(errorModel, 400, "Bad Request", "Request body failed serialization.");
                    var jsonResult = JsonSerializer.Serialize(errorResponse);
                    return new JsonResult(jsonResult);
                }

                var model = _sudokuRepo.AddPuzzle(puzzle);

                if (model == null)
                {
                    var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                    var errorResponse = new AddPuzzleResponsedto(errorModel, 409, "Conflict", "The database already contains a record with the given puzzle.");
                    var jsonResult = JsonSerializer.Serialize(errorResponse);
                    return new JsonResult(jsonResult);
                }

                var successResponse = new AddPuzzleResponsedto(model, 200, "OK", string.Empty);
                json = JsonSerializer.Serialize(successResponse);
                return new JsonResult(json);   
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SudokuController POST.  Error Message: {ex.Message}.  Inner Exception: {ex?.InnerException?.Message}");
                var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                var errorResponse = new AddPuzzleResponsedto(errorModel, 500, "Server Error", $"ErrorMessage: {ex?.Message} || {ex?.InnerException?.Message}");
                var json = JsonSerializer.Serialize(errorResponse);
                return new JsonResult(json);
            }
        }

        [HttpPost("DeletePuzzle")]
        [Route("/deletepuzzle")]
        public JsonResult DeletePuzzle([FromBody] string id)    
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                    var errorResponse = new AddPuzzleResponsedto(errorModel, 400, "Bad Request", "Request body is empty");
                    var jsonResult = JsonSerializer.Serialize(errorResponse);
                    return new JsonResult(jsonResult);
                }

                var guid = new SudokuPuzzledto(id);

                if (guid == null)
                {
                    var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                    var errorResponse = new AddPuzzleResponsedto(errorModel, 400, "Bad Request", "Request query string id cannot be parsed to guid.");
                    var jsonResult = JsonSerializer.Serialize(errorResponse);
                    return new JsonResult(jsonResult);
                }

                var model = _sudokuRepo.DeletePuzzle(guid);

                if (model == null)
                {
                    var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                    var errorResponse = new AddPuzzleResponsedto(errorModel, 404, "Not Found", "The database does not contain a record with the given ID.");
                    var jsonResult = JsonSerializer.Serialize(errorResponse);
                    return new JsonResult(jsonResult);
                }

                var successResponse = new AddPuzzleResponsedto(model, 200, "OK", string.Empty);
                var json = JsonSerializer.Serialize(successResponse);
                return new JsonResult(json);
            }
            catch (Exception ex)
            {
                var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                var errorResponse = new AddPuzzleResponsedto(errorModel, 500, "Server Error", $"ErrorMessage: {ex?.Message} || {ex?.InnerException?.Message}");
                var json = JsonSerializer.Serialize(errorResponse);
                return new JsonResult(json);
            }
        }
    }
}
