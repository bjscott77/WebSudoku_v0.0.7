namespace WebSudoku_v0._0._7.Configuration
{
    public class PlaySettings : GamePlaySettingsProperty
    {
        public PlaySettings() { }
        public PlaySettings(
            PuzzleSelectionMode? selection, 
            ONOFFPowerMode? cellStatisticsPowerMode,
            CellDisplayValueMode? cellDisplayValueType, 
            ONOFFPowerMode? hiLiteSolvedCell) : base(selection, cellStatisticsPowerMode, cellDisplayValueType, hiLiteSolvedCell)
        {
        }
    }
}
