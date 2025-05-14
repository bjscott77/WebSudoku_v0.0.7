namespace WebSudoku_v0._0._7.Configuration
{
    public class DevSudokuConfigurationSection : ILocalConfigurationSection
    {
        private readonly SudokuSettings? _sudokuSettings;
        [ConfigurationKeyName("Sudoku Settings")]
        public SudokuSettings? SudokuSettings { get; set; } = new SudokuSettings { };

        public DevSudokuConfigurationSection() { }

        public DevSudokuConfigurationSection(SudokuSettings? sudokuSettings)
        {
            SudokuSettings = sudokuSettings;
        }
    }
}
