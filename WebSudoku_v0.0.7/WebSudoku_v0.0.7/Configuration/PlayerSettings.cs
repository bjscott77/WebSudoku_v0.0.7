namespace WebSudoku_v0._0._7.Configuration
{
    public class PlayerSettings : GamePlaySettingsProperty
    {
        private readonly ONOFFPowerMode? _solveNextCell;
        [ConfigurationKeyName("SolveNextCell")]
        public ONOFFPowerMode? SolveNextCell { get; set; }

        private readonly ONOFFPowerMode? _useInGameTimer;
        [ConfigurationKeyName("UseInGameTimer")]
        public ONOFFPowerMode? UseInGameTimer { get; set; }

        private readonly ONOFFPowerMode? _useInGameScoring;
        [ConfigurationKeyName("UseInGameScoring")]
        public ONOFFPowerMode? UseInGameScoring { get; set; }

        private readonly CellValueChangeMode? _cellValueChangeType;
        [ConfigurationKeyName("CellValueChangeType")]
        public CellValueChangeMode? CellValueChangeType { get; set; }

        public PlayerSettings() { }

        public PlayerSettings(PuzzleSelectionMode? selection,
            ONOFFPowerMode? cellStatisticsPowerMode,
            CellDisplayValueMode? cellDisplayValueType,
            ONOFFPowerMode? hiLiteSolvedCell) :
            base(selection, cellStatisticsPowerMode, cellDisplayValueType, hiLiteSolvedCell)
        {
        }

        public PlayerSettings(PuzzleSelectionMode? selection,
            ONOFFPowerMode? cellStatisticsPowerMode,
            CellDisplayValueMode? cellDisplayValueType,
            ONOFFPowerMode? hiLiteSolvedCell,
            ONOFFPowerMode? solveNextCell,
            ONOFFPowerMode? usInGameTimer,
            ONOFFPowerMode? useInGameScoring,
            CellValueChangeMode? cellValueChangeType) :
            base(selection,
                cellStatisticsPowerMode,
                cellDisplayValueType,
                hiLiteSolvedCell)
        {
            SolveNextCell = solveNextCell;
            UseInGameTimer = usInGameTimer;
            UseInGameScoring = useInGameScoring;
            CellValueChangeType = cellValueChangeType;
        }
    }
}
