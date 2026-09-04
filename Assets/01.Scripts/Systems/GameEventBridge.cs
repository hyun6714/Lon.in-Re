using System;

public static class GameEventBridge
{
    public static event Action<Season> OnSeasonChanged;
    public static event Action OnSeasonEvent;
    public static event Action<bool> OnPausedChanged;

    public static void SeasonChanged(Season season) => OnSeasonChanged?.Invoke(season);
    public static void SeasonEvent() => OnSeasonEvent?.Invoke();
    public static void PausedChanged(bool isPaused) => OnPausedChanged?.Invoke(isPaused);
}
