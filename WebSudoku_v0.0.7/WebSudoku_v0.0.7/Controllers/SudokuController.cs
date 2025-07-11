﻿using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebSudoku_v0._0._7.Models;
using WebSudoku_v0._0._7.Repositories;
using WebSudoku_v0._0._7.Configuration;
using WebSudoku_v0._0._7.Classes;

namespace WebSudoku_v0._0._7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors()]
    public class SudokuController : ControllerBase
    {
        private readonly ILogger<SudokuController> _debugLogger;
        private readonly ISudokuRepository _sudokuRepo;
        private readonly DevConfiguration _devConfig;

        public SudokuController(ISudokuRepository sudokuRepo, DevConfiguration devConfig, ILogger<SudokuController> logger)
        {
            _debugLogger = logger;
            _sudokuRepo = sudokuRepo;
            _devConfig = devConfig;
        }

        [HttpGet("GetAllPuzzles")]
        [Route("/getallpuzzles")]
        public async Task<JsonResult> Get()
        {
            try
            {
                var model = await _sudokuRepo.GetAllPuzzlesAsync(); 

                if (model == null)
                {
                    var empty = _sudokuRepo.GetEmptyListReturnModel();
                    var errResonse = new SudokuResponse(empty, 404, "Not Found", "No puzzles found in the database.");
                    var mtModel = JsonSerializer.Serialize(errResonse);
                    return new JsonResult(mtModel);
                }

                var successResponse = new SudokuResponse(model, 200, "OK", string.Empty);
                successResponse = _sudokuRepo.UpdateSettings(successResponse, model[0].BoardValues);
                var json = JsonSerializer.Serialize(successResponse);
                return new JsonResult(json);
            }
            catch (Exception ex)
            {               
                _debugLogger.LogError($"SudokuController GET.  Error Message: {ex.Message}.  Inner Exception: {ex?.InnerException?.Message}");
                var model = _sudokuRepo.GetEmptyListReturnModel();
                var errorModel = new SudokuResponse(model, 500, "Server Error", $"Sudoku GetAllPuzzles: {ex?.Message}. Inner: {ex?.InnerException?.Message}");
                var json = JsonSerializer.Serialize(errorModel);
                return new JsonResult(json);
            }
        }

        [HttpGet("GetPuzzle")]
        [Route("/getpuzzle")]
        public async Task<JsonResult> GetSelected([FromQuery] string puzzle, [FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrEmpty(puzzle))
                {
                    var empty = _sudokuRepo.GetEmptyListReturnModel();
                    var errResonse = new SudokuResponse(empty, 400, "Bad Request", "Query string is malformed is null or empty.");
                    var mtJson = JsonSerializer.Serialize(errResonse);
                    return new JsonResult(mtJson);
                }

                var model = await _sudokuRepo.GetPuzzleAsync(puzzle, id);    

                if (model == null)
                {
                    var empty = _sudokuRepo.GetEmptyListReturnModel();
                    var errResonse = new SudokuResponse(empty, 404, "Not Found", "Selected puzzle was not found.");
                    var mtJson = JsonSerializer.Serialize(errResonse);
                    return new JsonResult(mtJson);
                }

                var successResponse = new SudokuResponse(model, 200, "OK", string.Empty);
                successResponse = _sudokuRepo.UpdateSettings(successResponse, model[0].BoardValues);
                var json = JsonSerializer.Serialize(successResponse);
                return new JsonResult(json);
            }
            catch (Exception ex)
            {
                _debugLogger.LogError($"SudokuController GETSelected.  Error Message: {ex.Message}.  Inner Exception: {ex?.InnerException?.Message}");
                var model = _sudokuRepo.GetEmptyListReturnModel();
                var errorModel = new SudokuResponse(model, 500, "Server Error", $"Sudoku Selected: {ex?.Message}. Inner: {ex?.InnerException?.Message}");
                var json = JsonSerializer.Serialize(errorModel);
                return new JsonResult(json);
            }
        }

        [HttpGet("GetSolvedPuzzle")]
        [Route("/getsolvedpuzzle")]
        public async Task<JsonResult> GetSolved([FromQuery] string puzzle)
        {
            try
            {
                if (string.IsNullOrEmpty(puzzle))
                {
                    var empty = _sudokuRepo.GetEmptyListReturnModel();
                    var errResonse = new SudokuResponse(empty, 400, "Bad Request", "Query string is malformed is null or empty.");
                    var mtJson = JsonSerializer.Serialize(errResonse);
                    return new JsonResult(mtJson);
                }

                var model = await _sudokuRepo.GetSolvedPuzzleAsync(puzzle);

                if (model == null)
                {
                    var empty = _sudokuRepo.GetEmptyListReturnModel();
                    var errResonse = new SudokuResponse(empty, 400, "Bad Request", "Unable to parse puzzle from entered text.");
                    var mtJson = JsonSerializer.Serialize(errResonse);
                    return new JsonResult(mtJson);
                }

                var successResponse = new SudokuResponse(model, 200, "OK", string.Empty);
                successResponse.CellDisplayValueType = _devConfig.SudokuSettings.GamePlaySettings.SolveSettings.CellDisplayValueType;
                var json = JsonSerializer.Serialize(successResponse);
                return new JsonResult(json);
            }
            catch (Exception ex)
            {
                _debugLogger.LogError($"SudokuController GETSolved.  Error Message: {ex.Message}.  Inner Exception: {ex?.InnerException?.Message}");
                var model = _sudokuRepo.GetEmptyListReturnModel();
                var errorModel = new SudokuResponse(model, 500, "Server Error", $"Sudoku GetSolved: {ex?.Message}. Inner: {ex?.InnerException?.Message}");
                var json = JsonSerializer.Serialize(errorModel);
                return new JsonResult(json);
            }
        }

        [HttpPost("AddPuzzle")]
        [Route("/addpuzzle")]
        public async Task<JsonResult> AddPuzzle()
        {
            try
            {
                if (_sudokuRepo == null)
                    return await Task.FromResult<JsonResult>(null);

                var json = await new StreamReader(Request.Body).ReadToEndAsync();
                if (string.IsNullOrEmpty(json))
                {
                    var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                    var errorResponse = new SudokuResponse(errorModel, 400, "Bad Request", "Request body is empty");
                    var jsonResult = JsonSerializer.Serialize(errorResponse);
                    return new JsonResult(jsonResult);
                }

                var puzzle = JsonSerializer.Deserialize<SudokuDTO>(json);
                if (puzzle == null)
                {
                    var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                    var errorResponse = new SudokuResponse(errorModel, 400, "Bad Request", "Request body is missing board values in the puzzle.");
                    var jsonResult = JsonSerializer.Serialize(errorResponse);
                    return new JsonResult(jsonResult);
                }
                if (string.IsNullOrEmpty(puzzle.BoardValues))
                {
                    var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                    var errorResponse = new SudokuResponse(errorModel, 400, "Bad Request", "Request body failed serialization.");
                    var jsonResult = JsonSerializer.Serialize(errorResponse);
                    return new JsonResult(jsonResult);
                }

                var model = await _sudokuRepo.AddPuzzleAsync(puzzle);

                if (model == null)
                {
                    var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                    var errorResponse = new SudokuResponse(errorModel, 409, "Conflict", "The database already contains a record with the given puzzle.");
                    var jsonResult = JsonSerializer.Serialize(errorResponse);
                    return new JsonResult(jsonResult);
                }

                var successResponse = new SudokuResponse(model, 200, "OK", string.Empty);
                successResponse.CellDisplayValueType = _devConfig.SudokuSettings.GamePlaySettings.SolveSettings.CellDisplayValueType;
                json = JsonSerializer.Serialize(successResponse);
                return new JsonResult(json);   
            }
            catch (Exception ex)
            {
                _debugLogger.LogError($"SudokuController POST.  Error Message: {ex.Message}.  Inner Exception: {ex?.InnerException?.Message}");
                var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                var errorResponse = new SudokuResponse(errorModel, 500, "Server Error", $"ErrorMessage: {ex?.Message} || {ex?.InnerException?.Message}");
                var json = JsonSerializer.Serialize(errorResponse);
                return new JsonResult(json);
            }
        }

        [HttpPut("UpdatePuzzle")]
        [Route("/updatepuzzle")]
        public async Task<JsonResult> UpdatePuzzle()
        {
            try
            {
                var json = await new StreamReader(Request.Body).ReadToEndAsync();
                if (string.IsNullOrEmpty(json))
                {
                    var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                    var errorResponse = new SudokuResponse(errorModel, 400, "Bad Request", "Request body is empty");
                    var jsonResult = JsonSerializer.Serialize(errorResponse);
                    return new JsonResult(jsonResult);
                }

                var puzzles = JsonSerializer.Deserialize<List<SudokuDTO>>(json);
                if (puzzles == null)
                {
                    var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                    var errorResponse = new SudokuResponse(errorModel, 400, "Bad Request", "Request body is missing board values in the puzzle.");
                    var jsonResult = JsonSerializer.Serialize(errorResponse);
                    return new JsonResult(jsonResult);
                }
                if (string.IsNullOrEmpty(puzzles[0].BoardValues))
                {
                    var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                    var errorResponse = new SudokuResponse(errorModel, 400, "Bad Request", "Request body failed serialization.");
                    var jsonResult = JsonSerializer.Serialize(errorResponse);
                    return new JsonResult(jsonResult);
                }

                var model = await _sudokuRepo.UpdatePuzzleAsync(puzzles);   

                if (model == null)
                {
                    var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                    var errorResponse = new SudokuResponse(errorModel, 404, "Not Found", "The puzzle could not be found to update.");
                    var jsonResult = JsonSerializer.Serialize(errorResponse);
                    return new JsonResult(jsonResult);
                }

                var successResponse = new SudokuResponse(model, 200, "OK", string.Empty);
                successResponse.CellDisplayValueType = _devConfig.SudokuSettings.GamePlaySettings.SolveSettings.CellDisplayValueType;
                json = JsonSerializer.Serialize(successResponse);
                return new JsonResult(json);
            }
            catch (Exception ex)
            {
                _debugLogger.LogError($"SudokuController POST.  Error Message: {ex.Message}.  Inner Exception: {ex?.InnerException?.Message}");
                var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                var errorResponse = new SudokuResponse(errorModel, 500, "Server Error", $"ErrorMessage: {ex?.Message} || {ex?.InnerException?.Message}");
                var json = JsonSerializer.Serialize(errorResponse);
                return new JsonResult(json);
            }
        }


        [HttpPost("DeletePuzzle")]
        [Route("/deletepuzzle")]
        public async Task<JsonResult> DeletePuzzle([FromBody] string puzzle)
        {
            try
            {
                if (string.IsNullOrEmpty(puzzle))
                {
                    var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                    var errorResponse = new SudokuResponse(errorModel, 400, "Bad Request", "Request body is empty");
                    var jsonResult = JsonSerializer.Serialize(errorResponse);
                    return new JsonResult(jsonResult);
                }

                var model = await _sudokuRepo.DeletePuzzleAsync(puzzle);

                if (model == null)
                {
                    var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                    var errorResponse = new SudokuResponse(errorModel, 404, "Not Found", "The database does not contain a record with the given ID.");
                    var jsonResult = JsonSerializer.Serialize(errorResponse);
                    return new JsonResult(jsonResult);
                }

                var successResponse = new SudokuResponse(model, 200, "OK", string.Empty);
                successResponse.CellDisplayValueType = _devConfig.SudokuSettings.GamePlaySettings.SolveSettings.CellDisplayValueType;
                var json = JsonSerializer.Serialize(successResponse);
                return new JsonResult(json);
            }
            catch (Exception ex)
            {
                _debugLogger.LogError($"SudokuController POST Delete.  Error Message: {ex.Message}.  Inner Exception: {ex?.InnerException?.Message}");
                var errorModel = _sudokuRepo.GetEmptyListReturnModel();
                var errorResponse = new SudokuResponse(errorModel, 500, "Server Error", $"ErrorMessage: {ex?.Message} || {ex?.InnerException?.Message}");
                var json = JsonSerializer.Serialize(errorResponse);
                return new JsonResult(json);
            }
        }
    }
}
