using System;

public static class GameEventBridge
{
    /// <summary> 날짜 변경시 발생 (월, 일) </summary>
    public static event Action<int, int> OnDayChanged;

    /// <summary> 7시 마다 이벤트 체크 </summary>
    public static event Action OnEventStarted;

    /// <summary> 게임이 일시정지 되었을 때 발생 </summary>
    public static event Action<bool> OnPausedChanged;

    /// <summary> 자동 생산 배수 변경 시 발생 (배율) </summary>
    public static event Action<float> OnAutoMultiplierChanged;

    /// <summary> 재화 증감 시 발생 (재화 타입, 재화량) </summary>
    public static event Action<CurrencyType, int> OnCurrencyAdded;    
    

    // 이벤트 실행
    public static void DayChanged(int month, int day) => OnDayChanged?.Invoke(month, day);
    public static void EventStarted() => OnEventStarted?.Invoke();
    public static void PausedChanged(bool isPaused) => OnPausedChanged?.Invoke(isPaused);
    public static void AutoMultiplierChanged(float multi) => OnAutoMultiplierChanged?.Invoke(multi);
    public static void CurrencyAdded(CurrencyType type, int value) => OnCurrencyAdded?.Invoke(type, value);
}
