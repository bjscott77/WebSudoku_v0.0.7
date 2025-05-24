namespace WebSudoku_v0._0._7.Classes
{
    public class PlaySettings
    {
        public string Selection { get; set; }
        public bool CellStatistics { get; set; }
        public string CellDisplayValueType { get; set; }
        public bool HiLiteSolvedCell { get; set; }

        public PlaySettings(IConfigurationSection devSettings)  
        {
            Selection = devSettings["Sudoku Settings:GamePlay:Play Settings:Selection"].ToString();
            CellStatistics = devSettings["Sudoku Settings:GamePlay:Play Settings:CellStatistics"].ToString() == "ON" ? true : false;
            CellDisplayValueType = devSettings["Sudoku Settings:GamePlay:Play Settings:CellDisplayValueType"].ToString();
            HiLiteSolvedCell = devSettings["Sudoku Settings:GamePlay:Play Settings:HiLiteSolvedCell"].ToString() == "ON" ? true : false;
        }
    }
}
