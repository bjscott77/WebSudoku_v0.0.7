namespace WebSudoku_v0._0._7.Classes
{
    public class SolveSettings
    {
        public string Selection { get; set; }
        public bool CellStatistics { get; set; }
        public string CellDisplayValueType { get; set; }
        public bool HiLiteSolvedCell { get; set; }

        public SolveSettings(IConfigurationSection devSettings) 
        {
            Selection = devSettings["Sudoku Settings:GamePlay:Solve Settings:Selection"].ToString();
            CellStatistics = devSettings["Sudoku Settings:GamePlay:Solve Settings:CellStatistics"].ToString() == "ON" ? true : false;
            CellDisplayValueType = devSettings["Sudoku Settings:GamePlay:Solve Settings:CellDisplayValueType"].ToString();
            HiLiteSolvedCell = devSettings["Sudoku Settings:GamePlay:Solve Settings:HiLiteSolvedCell"].ToString() == "ON" ? true : false;
        }
    }
}
