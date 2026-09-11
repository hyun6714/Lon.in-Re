using TMPro;
using UnityEngine;

public class ShopCurrencyHUD : MonoBehaviour
{
    [Header("Currency Texts")]
    [SerializeField] private TextMeshProUGUI normalCurrencyText;   // Normal_Currency_Text
    [SerializeField] private TextMeshProUGUI specialCurrencyText;  // Special_Currency_Text
    [SerializeField] private TextMeshProUGUI reputationText;       // Reputation_Text

    private void OnEnable()
    {
        SubscribeAndRefresh();
    }

    private void Start()
    {
        SubscribeAndRefresh();
    }

    private void OnDisable()
    {
        // 이벤트 해제
        if (CurrencyManager.instance != null)
        {
            CurrencyManager.instance.OnCurrencyChanged -= HandleCurrencyChanged;
        }
    }

    private void SubscribeAndRefresh()
    {
        if (CurrencyManager.instance == null) return;

        // 중복 구독 방지 후 이벤트 재연결
        CurrencyManager.instance.OnCurrencyChanged -= HandleCurrencyChanged;
        CurrencyManager.instance.OnCurrencyChanged += HandleCurrencyChanged;

        RefreshAllCurrencies();
    }

    private void HandleCurrencyChanged(CurrencyType type, int amount)
    {
        UpdateCurrencyText(type, amount);
    }

    // HUD 활성화 시 전체 1회 갱신
    public void RefreshAllCurrencies()
    {
        if (CurrencyManager.instance == null) return;

        UpdateCurrencyText(CurrencyType.Normal, CurrencyManager.instance.GetAmount(CurrencyType.Normal));
        UpdateCurrencyText(CurrencyType.Special, CurrencyManager.instance.GetAmount(CurrencyType.Special));
        UpdateCurrencyText(CurrencyType.Reputation, CurrencyManager.instance.GetAmount(CurrencyType.Reputation));
    }

    private void UpdateCurrencyText(CurrencyType type, int amount)
    {
        switch (type)
        {
            case CurrencyType.Normal:
                if (normalCurrencyText != null)
                    normalCurrencyText.text = $"일반 재화 : {CurrencyFormatter.Format(amount)}";
                break;

            case CurrencyType.Special:
                if (specialCurrencyText != null)
                    specialCurrencyText.text = $"특수 재화 : {CurrencyFormatter.Format(amount)}";
                break;

            case CurrencyType.Reputation:
                if (reputationText != null)
                    reputationText.text = $"명성 : {CurrencyFormatter.Format(amount)}";
                break;
        }
    }



    // 에디터 테스트용 치트키
#if UNITY_EDITOR
    [ContextMenu("Debug/일반 재화 +10,000")]
    private void DebugAddNormalCurrency()
    {
        if (CurrencyManager.instance != null)
        {
            CurrencyManager.instance.AddCurrency(CurrencyType.Normal, 10000);
            Debug.Log($"[디버그] 일반 재화 +10,000 지급! (현재: {CurrencyManager.instance.GetAmount(CurrencyType.Normal)})");
        }
    }

    [ContextMenu("Debug/특수 재화 +5000")]
    private void DebugAddSpecialCurrency()
    {
        if (CurrencyManager.instance != null)
        {
            CurrencyManager.instance.AddCurrency(CurrencyType.Special, 5000);
            Debug.Log($"[디버그] 특수 재화 +1000 지급! (현재: {CurrencyManager.instance.GetAmount(CurrencyType.Special)})");
        }
    }

    [ContextMenu("Debug/명성 +500")]
    private void DebugAddReputation()
    {
        if (CurrencyManager.instance != null)
        {
            CurrencyManager.instance.AddCurrency(CurrencyType.Reputation, 500);
            Debug.Log($"[디버그] 명성 +500 지급! (현재: {CurrencyManager.instance.GetAmount(CurrencyType.Reputation)})");
        }
    }

    [ContextMenu("Debug/환생 횟수 +1")]
    private void DebugAddRebirthCount()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.playerRebirthCount++;
            Debug.Log($"[디버그] 환생 횟수 +1! (현재: {GameManager.Instance.playerRebirthCount}회)");
        }
    }

    [ContextMenu("Debug/회사 등급(Rank) 1단계 상승")]
    private void DebugRankUp()
    {
        if (RankManager.instance != null)
        {
            int nextRank = (int)RankManager.instance.currentRank + 1;
            if (nextRank <= (int)RankManager.RankState.MajorPublisher)
            {
                RankManager.instance.currentRank = (RankManager.RankState)nextRank;

                Debug.Log($"[디버그] 회사 등급 상승 완료: {RankManager.instance.currentRank} (최대 인원: {RankManager.instance.maxEmployee})");
            }
            else
            {
                Debug.LogWarning("[디버그] 이미 최고 등급(MajorPublisher)입니다.");
            }
        }
    }

    [ContextMenu("Debug/모든 조건 프리패스 지급")]
    private void DebugAddAllForTest()
    {
        if (CurrencyManager.instance != null)
        {
            CurrencyManager.instance.AddCurrency(CurrencyType.Normal, 10000000);
            CurrencyManager.instance.AddCurrency(CurrencyType.Special, 10000);
            CurrencyManager.instance.AddCurrency(CurrencyType.Reputation, 3000);
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.playerRebirthCount += 5;
        }
        if (RankManager.instance != null)
        {
            RankManager.instance.currentRank = RankManager.RankState.MajorPublisher;
        }

        Debug.Log("[디버그] 모든 조건 프리패스 지급 완료! (일반 1000만, 특수 10000, 명성 3000, 환생 5회, 대기업 등급)");
    }
#endif
}