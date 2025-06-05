namespace WebSudoku_v0._0._7.Configuration
{
    public class SolveSettings
    {
        public string Selection { get; set; }
        public bool CellStatistics { get; set; }
        public string CellDisplayValueType { get; set; }
        public bool HiLiteSolvedCell { get; set; }
        public int MaxAttempts { get; set; }
        public bool ShowDebugInfo { get; set; }

        private readonly IConfigurationSection _devSettings;
        public SolveSettings(IConfigurationSection devSettings) 
        {
            _devSettings = devSettings;
            Selection = _devSettings["Sudoku Settings:GamePlay:Solve Settings:Selection"].ToString();
            CellStatistics = _devSettings["Sudoku Settings:GamePlay:Solve Settings:CellStatistics"].ToString() == "ON" ? true : false;
            CellDisplayValueType = _devSettings["Sudoku Settings:GamePlay:Solve Settings:CellDisplayValueType"].ToString();
            HiLiteSolvedCell = _devSettings["Sudoku Settings:GamePlay:Solve Settings:HiLiteSolvedCell"].ToString() == "ON" ? true : false;
            MaxAttempts = int.TryParse(_devSettings["Sudoku Settings:GamePlay:Solve Settings:MaxAttempts"], out int result) ? result : 0;
            ShowDebugInfo = _devSettings["Sudoku Settings:GamePlay:Solve Settings:ShowDebugInfo"] == "ON" ? true : false;
        }
    }
}
