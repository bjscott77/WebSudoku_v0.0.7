using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebSudoku_v0._0._2.Models;
using WebSudoku_v0._0._2.Repositories;

namespace WebSudoku_v0._0._7.Classes
{
    public class CustomRouting
    {
        private readonly IConfiguration _appConfig;
        private readonly ISudokuRepository _sudokuRepository;
        private ISudokuBoard _sudokuBoard;

        public CustomRouting(WebApplicationBuilder builder, ISudokuBoard board, ISudokuRepository sudokuRepository)
        {
            _sudokuBoard = board;
            _appConfig = builder.Configuration;
            _sudokuRepository = sudokuRepository;
        }

        public void UseCustomAPI(ref WebApplication app)
        {
            var title = string.Empty;
            var webEncoder = HtmlEncoder.Create(new TextEncoderSettings());
            var board = new SudokuBoard(new SudokuDimensions(81, 9));

            #region GetRoot
            ////  Root Page
            //app.MapGet("/", async (HttpContext context) =>
            //{
            //    title = "My Site";
            //    context.Response.Headers.ContentType = "text/html; charset=utf-8";
            //    await context.Response.WriteAsync($"<h1>Welcome to {title}!</h1><br/><a href='/index.html'>Sudoku Solver</a>");
            //});
            #endregion

            #region APIRoot
            //  API
            app.MapGet("/api", async (context) =>
            {
                title = "API";
                context.Response.Headers.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync($"<h1>Welcome to {title}!</h1>");
            });
            #endregion

            #region SudokuAPI
            //  Sudoku API

            var apiRoutes = _appConfig.GetSection("ApiRoutes");
            if (apiRoutes == null)
                throw new ArgumentNullException(nameof(apiRoutes));

            var key = "GetBoard";
            var url = apiRoutes.GetValue<string>(key);
            var apiRoute = webEncoder.Encode(url) ?? string.Empty;
            app.MapGet(apiRoute, async (context) =>
            {
                title = "Sudoku API";
                board.InitializeBoard();
                board.InitializeOdds();
                context.Response.Headers.ContentType = "application/json; charset=utf-8";
                var json = JsonSerializer.Serialize<IEnumerable<Cell>>(board.Cells.List);
                await context.Response.WriteAsync(json);
            });

            key = "ResetBoard";
            url = apiRoutes.GetValue<string>(key);
            apiRoute = webEncoder.Encode(url) ?? string.Empty;
            app.MapGet(apiRoute, async (context) =>
            {
                title = "Sudoku API";
                context.Response.Headers.ContentType = "application/json; charset=utf-8";
                var json = JsonSerializer.Serialize<IEnumerable<Cell>>(board.Cells.List);
                await context.Response.WriteAsync(json);
            });

            key = "SolveBoard";
            url = apiRoutes.GetValue<string>(key);
            apiRoute = webEncoder.Encode(url) ?? string.Empty;
            app.MapGet(apiRoute, async (context) =>
            {
                title = "Sudoku API";
                context.Response.Headers.ContentType = "application/json; charset=utf-8";
                board = board.SudokuManager.RunSolution(board);
                var json = JsonSerializer.Serialize(board.Cells.List);
                await context.Response.WriteAsync(json);
            });

            key = "GetPuzzles";
            url = apiRoutes.GetValue<string>(key);
            apiRoute = webEncoder.Encode(url) ?? string.Empty;
            app.MapGet(apiRoute, async (context) =>
            {
                title = "Sudoku API";
                context.Response.Headers.ContentType = "application/json; charset=utf-8";
                var json = JsonSerializer.Serialize(board.Puzzles);
                await context.Response.WriteAsync(json);
            });

            key = "AddPuzzle";
            url = apiRoutes.GetValue<string>(key);
            apiRoute = webEncoder.Encode(url) ?? string.Empty;
            app.MapPost(apiRoute, async (HttpContext context, [FromBody] object data) =>
            {
                var puzzle = Puzzle.ConvertToPuzzle(data);
                title = "Sudoku API";
                board.InitializeBoard(puzzle.PuzzleValues);
                board.InitializeOdds();

                var json = JsonSerializer.Serialize(_sudokuRepository.AddNewPuzzle(puzzle));
                context.Response.Headers.ContentType = "application/json; charset=utf-8";
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync(json);
            });

            key = "DeletePuzzle";
            url = apiRoutes.GetValue<string>(key);
            apiRoute = webEncoder.Encode(url) ?? string.Empty;
            app.MapPost(apiRoute, async (HttpContext context, [FromBody] Puzzle puzzle) =>
            {
                var model = _sudokuRepository.DeletePuzzle(puzzle);
                var json = JsonSerializer.Serialize(puzzle);
                context.Response.Headers.ContentType = "application/json; charset=utf-8";
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync(json);
            });
            #endregion

        }
    }
}
