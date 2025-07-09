using WebSudoku_v0._0._7.Configuration;
using WebSudoku_v0._0._7.Models;

namespace WebSudoku_v0._0._7.Classes
{
    public class SudokuAPIHelpers
    {
        private readonly DevConfiguration _devConfig;
        public SudokuAPIHelpers(DevConfiguration devConfig)
        {
            _devConfig = devConfig;
        }

        public SudokuResponse AttachSettings(SudokuResponse response)
        {
            response.PuzzleMode = _devConfig.SudokuSettings.Mode;
            if (_devConfig.SudokuSettings.Mode == "PLAY")
            {
                response.CellDisplayValueType = _devConfig.SudokuSettings.GamePlaySettings.PlaySettings.CellDisplayValueType;
            } else
            {
                response.CellDisplayValueType = _devConfig.SudokuSettings.GamePlaySettings.SolveSettings.CellDisplayValueType;
            }
            return response;
        }
    }
}
