namespace WebSudoku_v0._0._7.Configuration
{
    public class SudokuSettings
    {
        private readonly string[]? _modeOptions;
        [ConfigurationKeyName("ModeOptions")]
        public string[]? ModeOptions { get; set; }

        private readonly string? _mode;
        [ConfigurationKeyName("Mode")]
        public string? Mode { get; set; }

        private readonly string[]? _selectionOptions;
        [ConfigurationKeyName("SelectionOptions")]
        public string[]? SelectionOptions { get; set; }

        private readonly string[]? _displayValueTypeOptions;
        [ConfigurationKeyName("DisplayValueTypeOptions")]
        public string[]? DisplayValueTypeOptions { get; set; }

        private readonly string[]? _solvedCellValueOptions;
        [ConfigurationKeyName("SolvedCellValueOptions")]
        public string[]? SolvedCellValueOptions { get; set; }

        private readonly string[]? _cellValueChangeOptions;
        [ConfigurationKeyName("CellValueChangeOptions")]
        public string[]? CellValueChangeOptions { get; set; }

        private readonly string? _cellStatisticsInitial;
        [ConfigurationKeyName("CellStatisticsInitial")]
        public string? CellStatisticsInitial { get; set; }

        private readonly GamePlaySettings? _gamePlay;
        [ConfigurationKeyName("GamePlay")]
        public GamePlaySettings? GamePlay { get; set; }

        public SudokuSettings()
        {
            
        }

        public SudokuSettings(
            string[]? modeOptions, 
            string? mode, 
            string[]? selectionOptions, 
            string[]? displayValueTypeOptions, 
            string[]? solvedCellValueOptions, 
            string[]? cellValueChangeOptions, 
            string? cellStatisticsInitial)
        {
            ModeOptions = modeOptions;
            Mode = mode;
            SelectionOptions = selectionOptions;
            DisplayValueTypeOptions = displayValueTypeOptions;
            SolvedCellValueOptions = solvedCellValueOptions;
            CellValueChangeOptions = cellValueChangeOptions;
            CellStatisticsInitial = cellStatisticsInitial;
        }
    }
}
