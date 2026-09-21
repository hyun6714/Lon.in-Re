using System;

public static class GameEventBridge
{
    /// <summary> 날짜 변경시 발생 (월, 일) </summary>
    public static event Action<GameDate> OnDayChanged;

    public static event Action<GameDate> OnTimeChanged;

    /// <summary> 이벤트 체크 </summary>
    public static event Action OnEventStarted;

    /// <summary> 게임이 일시정지 되었을 때 발생 </summary>
    public static event Action<bool> OnPausedChanged;

    /// <summary> 자동 생산 배수 변경 시 발생 (배율) </summary>
    public static event Action<GameEventType, float> OnAutoMultiplierChanged;

    /// <summary> 탭/클릭 배수 변경 시 발생 (배율) </summary>
    public static event Action<GameEventType, float> OnTapMultiplierChanged;

    /// <summary> 재화 획득 시 발생 (재화 타입, 재화량) </summary>
    public static event Action<CurrencyType, int> OnCurrencyAdded;

    /// <summary> 재화 사용 시 발생 (재화 타입, 재화량) </summary>
    public static event Func<CurrencyType, int, bool> OnCurrencyUsed;

    /// <summary> 재화량 변화 시 발생 </summary>
    public static event Action<CurrencyType, int> OnCurrencyChanged;

    /// <summary> 직원 수 변화 시 발생 </summary>
    public static event Action OnEmployeeChaned;

    /// <summary> 랭크 변화 시 발생 </summary>
    public static event Action OnRankChanged;

    /// <summary> 아티팩트 언락 시 발생 </summary>
    public static event Action OnArtifactUnlocked;

    /// <summary> 환생 시 발생 </summary>
    public static event Action OnReincarnated;

    /// <summary> 직원 수 텍스트 변경 용 델리게이트 </summary>
    public static event Action<int, int> OnEmployeeCountChanged;

    /// <summary> 팝업창 열 때 발생 </summary>
    public static event Action<UIName> OnPopupOpened;

    /// <summary> 팝업창 닫을 때 발생 </summary>
    public static event Action<UIName> OnPopupClosed;

    /// <summary> 다음 이벤트 남은시간 텍스트 갱신 용 델리게이트 </summary>
    public static event Action<GameDate> OnNextEventChanged;


    // 이벤트 실행
    public static void DayChanged(GameDate date) => OnDayChanged?.Invoke(date);
    public static void TimeChanged(GameDate date) => OnTimeChanged?.Invoke(date);
    public static void EventStarted() => OnEventStarted?.Invoke();
    public static void PausedChanged(bool isPaused) => OnPausedChanged?.Invoke(isPaused);
    public static void AutoMultiplierChanged(GameEventType type, float multi) => OnAutoMultiplierChanged?.Invoke(type, multi);
    public static void TapMultiplierChanged(GameEventType type, float multi) => OnTapMultiplierChanged?.Invoke(type, multi);
    public static void CurrencyAdded(CurrencyType type, int value) => OnCurrencyAdded?.Invoke(type, value);
    public static void CurrencyUsed(CurrencyType type, int value) => OnCurrencyUsed?.Invoke(type, value);
    public static void CurrencyChanged(CurrencyType type, int value) => OnCurrencyChanged?.Invoke(type, value);
    public static void EmployeeChanged() => OnEmployeeChaned?.Invoke();
    public static void RankChanged() => OnRankChanged?.Invoke();
    public static void ArtifactUnlocked() => OnArtifactUnlocked?.Invoke();

    public static void Reincarnated() => OnReincarnated?.Invoke();
    public static void EmployeeCountChanged(int current, int max) => OnEmployeeCountChanged?.Invoke(current, max);
    public static void PopupOpened(UIName name) => OnPopupOpened?.Invoke(name);
    public static void PopupClosed(UIName name) => OnPopupClosed?.Invoke(name);

    public static void NextEventChanged(GameDate nextEventDate) => OnNextEventChanged?.Invoke(nextEventDate);
}
