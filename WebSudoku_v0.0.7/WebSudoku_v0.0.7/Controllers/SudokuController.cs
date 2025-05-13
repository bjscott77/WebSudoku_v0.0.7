using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebSudoku_v0._0._7.Data;
using WebSudoku_v0._0._7.Models;

namespace WebSudoku_v0._0._7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SudokuController(AppDbContext _appDbContext) : ControllerBase
    {
        [HttpGet("GetAllPuzzles")]
        [Route("/getallpuzzles")]
        public JsonResult Get()
        {
            DtoSudokuPuzzleList puzzles = new DtoSudokuPuzzleList()
            {
                Puzzles = new DtoSudokuPuzzle[]
                {
                    new DtoSudokuPuzzle
                    {
                        Id = Guid.NewGuid(),
                        BoardValues = "350602004007040013069831007503000096000300745946000800692400008800703000004020001",
                        Difficulty = 2
                    },
                    new DtoSudokuPuzzle
                    {
                        Id = Guid.NewGuid(),
                        BoardValues = "000590037079032080008700000300001620090006370720300158000007865087010240054600700",
                        Difficulty = 2
                    }
                }
            };

            //return new JsonResult(model);
            var model = JsonSerializer.Serialize(puzzles);
            return new JsonResult(model);
        }    
    }
}
