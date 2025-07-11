using WebSudoku_v0._0._7.Configuration;
using WebSudoku_v0._0._7.Models;

namespace WebSudoku_v0._0._7.Classes
{
    public class SudokuAPIHelpers
    {
        private readonly DevConfiguration _devConfig;
        private readonly ISudokuManager _sudokuManager;
        private readonly ISudokuBoard _sudokuBoard;
        public SudokuAPIHelpers(DevConfiguration devConfig, ISudokuManager sudokuManager, ISudokuBoard sudokuBoard)
        {
            _devConfig = devConfig;
            _sudokuManager = sudokuManager;
            _sudokuBoard = sudokuBoard;
        }

        public SudokuResponse AttachSettings(SudokuResponse response, string boardValues)
        {
            response.PuzzleMode = _devConfig.SudokuSettings.Mode;
            if (_devConfig.SudokuSettings.Mode == "PLAY")
            {
                response.CellDisplayValueType = _devConfig.SudokuSettings.GamePlaySettings.PlaySettings.CellDisplayValueType;
                _sudokuBoard.createSudokuBoard(boardValues);
                _sudokuBoard.InitializeProbabilities();
                if (_sudokuManager.CompleteBoard(_sudokuBoard.GetCells())
                    && _sudokuManager.IsBoardValid(_sudokuBoard.GetCells(), false))
                {
                    response.Solved = true;
                } else
                {
                    response.Solved = false;
                }
            } else
            {
                response.CellDisplayValueType = _devConfig.SudokuSettings.GamePlaySettings.SolveSettings.CellDisplayValueType;
            }
            return response;
        }
    }
}
