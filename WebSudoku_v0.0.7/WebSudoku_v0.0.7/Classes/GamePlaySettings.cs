namespace WebSudoku_v0._0._7.Classes
{
    public class GamePlaySettings
    {
        public PlaySettings PlaySettings { get; set; }
        public SolveSettings SolveSettings { get; set; }
        public PlayerSettings PlayerSettings { get; set; }

        private readonly IConfigurationSection _devSettings;
        public GamePlaySettings(IConfigurationSection devSettings)  
        {
            _devSettings = devSettings;
            PlaySettings = new PlaySettings(_devSettings);
            SolveSettings = new SolveSettings(_devSettings);
            PlayerSettings = new PlayerSettings(_devSettings);

        }
    }
}
