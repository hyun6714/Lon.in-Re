using System;

public static class GameEventBridge
{
    /// <summary> 날짜 변경시 발생 (월, 일) </summary>
    public static event Action<GameDate> OnDayChanged;

    public static event Action<UIName, GameDate> OnTimeChanged;

    /// <summary> 7시 마다 이벤트 체크 </summary>
    public static event Action OnEventStarted;

    /// <summary> 게임이 일시정지 되었을 때 발생 </summary>
    public static event Action<bool> OnPausedChanged;

    /// <summary> 자동 생산 배수 변경 시 발생 (배율) </summary>
    public static event Action<float> OnAutoMultiplierChanged;

    /// <summary> 재화 획득 시 발생 (재화 타입, 재화량) </summary>
    public static event Action<CurrencyType, int> OnCurrencyAdded;

    /// <summary> 재화 사용 시 발생 (재화 타입, 재화량) </summary>
    public static event Func<CurrencyType, int, bool> OnCurrencyUsed;

    /// <summary> 재화량 변화 시 발생 </summary>
    public static event Action<CurrencyType, int> OnCurrencyChanged;
    

    // 이벤트 실행
    public static void DayChanged(GameDate date) => OnDayChanged?.Invoke(date);
    public static void TimeChanged(UIName name, GameDate date) => OnTimeChanged?.Invoke(name, date);
    public static void EventStarted() => OnEventStarted?.Invoke();
    public static void PausedChanged(bool isPaused) => OnPausedChanged?.Invoke(isPaused);
    public static void AutoMultiplierChanged(float multi) => OnAutoMultiplierChanged?.Invoke(multi);
    public static void CurrencyAdded(CurrencyType type, int value) => OnCurrencyAdded?.Invoke(type, value);
    public static void CurrencyUsed(CurrencyType type, int value) => OnCurrencyUsed?.Invoke(type, value);
    public static void CurrencyChanged(CurrencyType type, int value) => OnCurrencyChanged?.Invoke(type, value);
}
