using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebSudoku_v0._0._7.Data;

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
            string[] puzzles = new string[]
            {
                "350602004007040013069831007503000096000300745946000800692400008800703000004020001",
                "000590037079032080008700000300001620090006370720300158000007865087010240054600700",
            };

            //return new JsonResult(model);
            var model = JsonSerializer.Serialize(puzzles);
            return new JsonResult(model);
        }    
    }
}
