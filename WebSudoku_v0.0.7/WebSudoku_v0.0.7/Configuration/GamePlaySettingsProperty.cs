namespace WebSudoku_v0._0._7.Configuration
{
    public  class GamePlaySettingsProperty : IGamePlaySettingsProperty
    {
        public enum PuzzleSelectionMode
        {
            NEW = 0,
            SELECTED = 1,
            RANDOM = 2,
            SEQUENTIAL = 3,
            SOLVED = 4,

        }
        public enum CellValueChangeMode
        {
            Click = 0,
            Text = 1
        }
        public enum ONOFFPowerMode
        {
            OFF = 0,
            ON = 1
        }
        public enum CellDisplayValueMode
        {
            SPACE = 0,
            NUM = 1
        }

        private readonly PuzzleSelectionMode _selection;
        [ConfigurationKeyName("Selection")]
        public PuzzleSelectionMode? Selection { get; set; }

        private readonly ONOFFPowerMode _cellStatisticsPowerMode;
        [ConfigurationKeyName("CellStatisticsPowerMode")]
        public ONOFFPowerMode? CellStatisticsPowerMode { get; set; }

        private readonly CellDisplayValueMode _cellDisplayValueType;
        [ConfigurationKeyName("CellDisplayValueType")]
        public CellDisplayValueMode? CellDisplayValueType { get; set; }

        private readonly ONOFFPowerMode _hiLiteSolvedCell;
        [ConfigurationKeyName("HiLiteSolvedCell")]
        public ONOFFPowerMode? HiLiteSolvedCell { get; set; }

        public GamePlaySettingsProperty()
        {
            
        }

        public GamePlaySettingsProperty(
            PuzzleSelectionMode? selection, 
            ONOFFPowerMode? cellStatisticsPowerMode, 
            CellDisplayValueMode? cellDisplayValueType, 
            ONOFFPowerMode? hiLiteSolvedCell)
        {
            Selection = selection;
            CellStatisticsPowerMode = cellStatisticsPowerMode;
            CellDisplayValueType = cellDisplayValueType;
            HiLiteSolvedCell = hiLiteSolvedCell;
        }
    }
}
