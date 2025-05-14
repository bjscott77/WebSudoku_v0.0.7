namespace WebSudoku_v0._0._7.Configuration
{
    public class GamePlaySettings
    {
        private readonly string[]? _newConfigPuzzles;
        [ConfigurationKeyName("New Config Puzzles")]
        public string[]? NewConfigPuzzles { get; set; } = new string[] { };

        private readonly IGamePlaySettingsProperty? _playSettings;
        [ConfigurationKeyName("Play Settings")]
        public IGamePlaySettingsProperty? PlaySettings { get; set; }

        private readonly IGamePlaySettingsProperty? _solveSettings;
        [ConfigurationKeyName("Solve Settings")]
        public IGamePlaySettingsProperty? SolveSettings { get; set; }

        private readonly IGamePlaySettingsProperty? _playerSettings;
        [ConfigurationKeyName("Player Settings")]
        public IGamePlaySettingsProperty? PlayerSettings { get; set; }

        public GamePlaySettings()
        {

        }

        public GamePlaySettings(
            string[] newConfigPuzzles, 
            IGamePlaySettingsProperty? playSettings, 
            IGamePlaySettingsProperty? solveSettings,
            IGamePlaySettingsProperty? playerSettings)
        {
            NewConfigPuzzles = newConfigPuzzles;
            PlaySettings = playSettings;
            SolveSettings = solveSettings;
            PlayerSettings = playerSettings;
        }
    }
}

