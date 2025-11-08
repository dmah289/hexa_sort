namespace Framework
{
    public static class KeySave
    {
        public static string PoolParentName = "PoolParent";
        
        // PlayerPrefs keys
        public static string LevelIndexKey = "LevelIndex";
        
        // Life System keys
        public static string maxKey = "MAX";
        public static string curLifeKey = "CurLife";
        public static string lastSaveTimeKey = "LastSaveTime";
        public static string lastCountdownRemaining = "TimeRemaining";
        
        public static string CoinCountKey = "CoinCount";
        
        // Settings keys
        public static string[] SettingsKeys =
        {
            "Music",
            "SFX",
            "Vibration"
        };
    }
}