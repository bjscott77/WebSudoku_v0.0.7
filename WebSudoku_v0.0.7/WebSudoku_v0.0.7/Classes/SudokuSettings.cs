namespace WebSudoku_v0._0._7.Classes
{
    public class SudokuSettings
    {
        public IEnumerable<int> BoardDimensions { get; set; }
        public IEnumerable<string> ModeOptions { get; set; }
        public string Mode { get; set; }
        public IEnumerable<string> SelectionOptions { get; set; }
        public IEnumerable<string> DisplayValueTypeOptions { get; set; }
        public IEnumerable<string> SolvedCellValueOptions { get; set; }
        public IEnumerable<string> CellValueChangeOptions { get; set; }
        public IEnumerable<int> CellStatisticsInitial { get; set; }
        public IEnumerable<int> CellStatisticsEmpty { get; set; }
        public GamePlaySettings GamePlaySettings { get; set; }

        private readonly IConfigurationSection _devSettings;
        public SudokuSettings(IConfigurationSection devSettings)
        {
            _devSettings = devSettings;
            BoardDimensions = _devSettings["Sudoku Settings:Board Dimensions"].Split(',').Select(d => int.Parse(d));
            ModeOptions = _devSettings["Sudoku Settings:ModeOptions"].Split(',');
            Mode = _devSettings["Sudoku Settings:Mode"].ToString();
            SelectionOptions = _devSettings["Sudoku Settings:SelectionOptions"].Split(',');
            DisplayValueTypeOptions = _devSettings["Sudoku Settings:DisplayValueTypeOptions"].Split(',');
            SolvedCellValueOptions = _devSettings["Sudoku Settings:SolvedCellValueOptions"].Split(',');
            CellValueChangeOptions = _devSettings["Sudoku Settings:CellValueChangeOptions"].Split(',');
            CellStatisticsInitial = _devSettings["Sudoku Settings:CellStatisticsInitial"].Split(',').Select(s => int.Parse(s));
            CellStatisticsEmpty = _devSettings["Sudoku Settings:CellStatisticsEmpty"].Split(',').Select(s => int.Parse(s));
            GamePlaySettings = new GamePlaySettings(_devSettings);
        }
    }
}
