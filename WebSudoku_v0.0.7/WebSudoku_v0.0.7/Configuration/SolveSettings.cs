namespace WebSudoku_v0._0._7.Configuration
{
    public class SolveSettings : GamePlaySettingsProperty
    {
        public SolveSettings() { }
        public SolveSettings(
            PuzzleSelectionMode? selection,
            ONOFFPowerMode? cellStatisticsPowerMode,
            CellDisplayValueMode? cellDisplayValueType,
            ONOFFPowerMode? hiLiteSolvedCell) : base(selection, cellStatisticsPowerMode, cellDisplayValueType, hiLiteSolvedCell)
        {
        }
    }
}
