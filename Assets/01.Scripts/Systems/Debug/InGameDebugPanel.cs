using UnityEngine;
using UnityEngine.InputSystem;

public class InGameDebugPanel : MonoBehaviour
{
    [Header("디버그 패널 UI 오브젝트")]
    [SerializeField] private GameObject debugPanelRoot;

    // 날짜 제어용 변수
    private int setYear = 1;
    private int setMonth = 3;
    private int setDay = 1;
    private int setHour = 7;

    private float timeDefaultScale = 1f;
    private float timeMultiScale = 30f;

    // 재화 입력값을 담을 변수들 (기본값 설정)
    private int inputNormalCurrency = 10000;
    private int inputSpecialCurrency = 10000;
    private int inputReputation = 10000;

    private void Update()
    {
        // f12 키로 디버그 패널 켜고 끄기
        if (Keyboard.current.f12Key.wasPressedThisFrame)
        {
            if (debugPanelRoot != null)
            {
                debugPanelRoot.SetActive(!debugPanelRoot.activeSelf);
            }
        }
    }

    private void Awake()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (debugPanelRoot != null) debugPanelRoot.SetActive(false);
#else
        if (debugPanelRoot != null)
            Destroy(debugPanelRoot);

        Destroy(this);
#endif
    }

    #region 시간 제어
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public void OnClickPause() => GameManager.instance?.GamePaused();
    public void OnClickResume() => GameManager.instance?.GameResume();
    public void OnClickAccelerate() => Time.timeScale = timeMultiScale;
    public void OnClickResetTimeScale() => Time.timeScale = timeDefaultScale;
#endif
    #endregion

    #region 날짜 변경 기능
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public void SetInputYear(string value) { if (int.TryParse(value, out int result)) setYear = result; }
    public void SetInputMonth(string value) { if (int.TryParse(value, out int result)) setMonth = result; }
    public void SetInputDay(string value) { if (int.TryParse(value, out int result)) setDay = result; }
    public void SetInputHour(string value) { if (int.TryParse(value, out int result)) setHour = result; }

    public void OnClickApplyDate()
    {
        if (CalendarManager.instance != null)
        {
            GameDate date = new GameDate()
            {
                year = setYear,
                month = setMonth,
                day = setDay,
                hour = setHour
            };
            CalendarManager.instance.SetDateOnlyEditor(date);
            Debug.Log($"[DebugPanel] 날짜 변경 완료: {setYear}년 {setMonth}월 {setDay}일 {setHour}시");
        }
    }
#endif
    #endregion

    #region 재화 입력 및 획득/감소 기능
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    // UI InputField의 On Value Changed 이벤트에 연결할 함수들
    public void SetInputNormalCurrency(string value) 
    {
        if (int.TryParse(value, out int result))
            inputNormalCurrency = result; 
    }

    public void SetInputSpecialCurrency(string value)
    {
        if (int.TryParse(value, out int result)) 
            inputSpecialCurrency = result;
    }

    public void SetInputReputation(string value)
    {
        if (int.TryParse(value, out int result))
            inputReputation = result; 
    }

    // 실제 버튼 클릭 이벤트에 연결할 함수들
    public void OnClickAddNormalCurrency() => GameEventBridge.CurrencyAdded(CurrencyType.Normal, inputNormalCurrency);
    public void OnClickUseNormalCurrency() => GameEventBridge.CurrencyUsed(CurrencyType.Normal, inputNormalCurrency);

    public void OnClickAddSpecialCurrency() => GameEventBridge.CurrencyAdded(CurrencyType.Special, inputSpecialCurrency);
    public void OnClickUseSpecialCurrency() => GameEventBridge.CurrencyUsed(CurrencyType.Special, inputSpecialCurrency);

    public void OnClickAddReputation() => GameEventBridge.CurrencyAdded(CurrencyType.Reputation, inputReputation);
    public void OnClickUseReputation() => GameEventBridge.CurrencyUsed(CurrencyType.Reputation, inputReputation);
#endif
    #endregion

    #region 데이터 초기화
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public void OnClickResetSaveData()
    {
        SaveManager.instance?.DeleteSaveData();
        Debug.Log("[DebugPanel] 세이브 데이터 초기화 완료");
    }
#endif
    #endregion
}
