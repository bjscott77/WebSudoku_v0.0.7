namespace WebSudoku_v0._0._7.Classes
{
    public class DevConfiguration
    {
        public SudokuSettings SudokuSettings { get; set; } = null;
        private readonly IConfigurationSection _devConfig;
        public DevConfiguration(IConfigurationSection devConfig)
        {
            _devConfig = devConfig;
            SudokuSettings = new SudokuSettings(_devConfig);
        }
    }
}
