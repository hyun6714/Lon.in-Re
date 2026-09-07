using System;

public static class GameEventBridge
{
    public static event Action<int, int> OnDayChanged;
    public static event Action OnEventStarted;
    public static event Action<bool> OnPausedChanged;
    public static event Action<float> OnAutoMultiplierChanged;

    public static void DayChanged(int month, int day) => OnDayChanged?.Invoke(month, day);
    public static void EventStarted() => OnEventStarted?.Invoke();
    public static void PausedChanged(bool isPaused) => OnPausedChanged?.Invoke(isPaused);
    public static void AutoMultiplierChanged(float multi) => OnAutoMultiplierChanged?.Invoke(multi);
}
