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

        public SudokuSettings(IConfigurationSection devSettings)
        {
            BoardDimensions = devSettings["Sudoku Settings:Board Dimensions"].Split(',').Select(d => int.Parse(d));
            ModeOptions = devSettings["Sudoku Settings:ModeOptions"].Split(',');
            Mode = devSettings["Sudoku Settings:Mode"].ToString();
            SelectionOptions = devSettings["Sudoku Settings:SelectionOptions"].Split(',');
            DisplayValueTypeOptions = devSettings["Sudoku Settings:DisplayValueTypeOptions"].Split(',');
            SolvedCellValueOptions = devSettings["Sudoku Settings:SolvedCellValueOptions"].Split(',');
            CellValueChangeOptions = devSettings["Sudoku Settings:CellValueChangeOptions"].Split(',');
            CellStatisticsInitial = devSettings["Sudoku Settings:CellStatisticsInitial"].Split(',').Select(s => int.Parse(s));
            CellStatisticsEmpty = devSettings["Sudoku Settings:CellStatisticsEmpty"].Split(',').Select(s => int.Parse(s));
            GamePlaySettings = new GamePlaySettings(devSettings);
        }
    }
}
