namespace WebSudoku_v0._0._7.Classes
{
    public class DevConfiguration
    {
        public SudokuSettings SudokuSettings { get; set; } = null;
        public IConfigurationSection DevConfig { get; set; } = null;
        public DevConfiguration(IConfigurationSection _devConfig)
        {
            SudokuSettings = new SudokuSettings(_devConfig);
        }
    }
}
