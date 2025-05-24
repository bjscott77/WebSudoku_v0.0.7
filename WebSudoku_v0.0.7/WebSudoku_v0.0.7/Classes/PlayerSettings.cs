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

        public PlayerSettings(IConfigurationSection devSettings)    
        {
            CellStatistics = devSettings["Sudoku Settings:GamePlay:Player Settings:CellStatistics"].ToString() == "ON" ? true : false;
            CellDisplayValueType = devSettings["Sudoku Settings:GamePlay:Player Settings:CellDisplayValueType"].ToString();
            HiLiteSolvedCell = devSettings["Sudoku Settings:GamePlay:Player Settings:HiLiteSolvedCell"].ToString() == "ON" ? true : false;
            SolveNextCell = devSettings["Sudoku Settings:GamePlay:Player Settings:SolveNextCell"].ToString() == "ON" ? true : false;
            UseInGameTimer = devSettings["Sudoku Settings:GamePlay:Player Settings:UseInGameTimer"].ToString() == "ON" ? true : false;
            UseInGameScoring = devSettings["Sudoku Settings:GamePlay:Player Settings:UseInGameScoring"].ToString() == "ON" ? true : false;
            CellValueChangeMode = devSettings["Sudoku Settings:GamePlay:Player Settings:CellValueChangeMode"].ToString();
        }
    }
}
