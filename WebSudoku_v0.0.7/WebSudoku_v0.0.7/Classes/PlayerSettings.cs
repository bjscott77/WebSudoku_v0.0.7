namespace WebSudoku_v0._0._7.Classes
{
    public class PlayerSettings
    {
        public bool CellStatistics { get; set; }
        public string CellDisplayValueType { get; set; }
        public bool HiLiteSolvedCell { get; set; }
        public bool SolveNextCell { get; set; }
        public bool UseInGameTimer { get; set; }
        public bool UseInGameScoring { get; set; }
        public string CellValueChangeMode { get; set; }

        private readonly IConfigurationSection _devSettings;
        public PlayerSettings(IConfigurationSection devSettings)    
        {
            _devSettings = devSettings;
            CellStatistics = _devSettings["Sudoku Settings:GamePlay:Player Settings:CellStatistics"].ToString() == "ON" ? true : false;
            CellDisplayValueType = _devSettings["Sudoku Settings:GamePlay:Player Settings:CellDisplayValueType"].ToString();
            HiLiteSolvedCell = _devSettings["Sudoku Settings:GamePlay:Player Settings:HiLiteSolvedCell"].ToString() == "ON" ? true : false;
            SolveNextCell = _devSettings["Sudoku Settings:GamePlay:Player Settings:SolveNextCell"].ToString() == "ON" ? true : false;
            UseInGameTimer = _devSettings["Sudoku Settings:GamePlay:Player Settings:UseInGameTimer"].ToString() == "ON" ? true : false;
            UseInGameScoring = _devSettings["Sudoku Settings:GamePlay:Player Settings:UseInGameScoring"].ToString() == "ON" ? true : false;
            CellValueChangeMode = _devSettings["Sudoku Settings:GamePlay:Player Settings:CellValueChangeMode"].ToString();
        }
    }
}
