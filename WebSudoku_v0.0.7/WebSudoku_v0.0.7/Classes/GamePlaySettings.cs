namespace WebSudoku_v0._0._7.Classes
{
    public class GamePlaySettings
    {
        public PlaySettings PlaySettings { get; set; }
        public SolveSettings SolveSettings { get; set; }
        public PlayerSettings PlayerSettings { get; set; }
        public GamePlaySettings(IConfigurationSection devSettings)  
        {
            PlaySettings = new PlaySettings(devSettings);
            SolveSettings = new SolveSettings(devSettings);
            PlayerSettings = new PlayerSettings(devSettings);
        }
    }
}
