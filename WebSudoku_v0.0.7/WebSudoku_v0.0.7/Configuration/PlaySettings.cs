namespace WebSudoku_v0._0._7.Configuration
{
    public class PlaySettings
    {
        public string Selection { get; set; }
        public bool CellStatistics { get; set; }
        public string CellDisplayValueType { get; set; }
        public bool HiLiteSolvedCell { get; set; }

        private readonly IConfigurationSection _devSettings;
        public PlaySettings(IConfigurationSection devSettings)  
        {
            _devSettings = devSettings;
            Selection = _devSettings["Sudoku Settings:GamePlay:Play Settings:Selection"].ToString();
            CellStatistics = _devSettings["Sudoku Settings:GamePlay:Play Settings:CellStatistics"].ToString() == "ON" ? true : false;
            CellDisplayValueType = _devSettings["Sudoku Settings:GamePlay:Play Settings:CellDisplayValueType"].ToString();
            HiLiteSolvedCell = _devSettings["Sudoku Settings:GamePlay:Play Settings:HiLiteSolvedCell"].ToString() == "ON" ? true : false;
        }
    }
}
