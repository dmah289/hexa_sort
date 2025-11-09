namespace HexaSort.Scripts.Managers
{
    public enum eLevelState : byte
    {
        None = 0,
        Playing = 1,
        Paused = 2,
        Completed = 3,
        Failed = 4,
        OutOfSpace = 5
    }
}