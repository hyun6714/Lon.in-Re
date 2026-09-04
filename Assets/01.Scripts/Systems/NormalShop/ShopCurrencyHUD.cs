using TMPro;
using UnityEngine;

public class ShopCurrencyHUD : MonoBehaviour
{
    [Header("Currency Texts")]
    [SerializeField] private TextMeshProUGUI normalCurrencyText;   // Normal_Currency_Text
    [SerializeField] private TextMeshProUGUI specialCurrencyText;  // Special_Currency_Text
    [SerializeField] private TextMeshProUGUI reputationText;       // Reputation_Text

    // 이전 수치 캐싱 (최초 1회 갱신)
    private int prevNormal = -1;
    private int prevSpecial = -1;
    private int prevReputation = -1;

    private void OnEnable()
    {
        prevNormal = -1;
        prevSpecial = -1;
        prevReputation = -1;

        InvokeRepeating(nameof(RefreshCurrencyUI), 0f, 0.2f);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(RefreshCurrencyUI));
    }

    public void RefreshCurrencyUI()
    {
        if (CurrencyManager.instance == null)
        {
            return;
        }

        int normal = CurrencyManager.instance.GetAmount(CurrencyType.Normal);
        int special = CurrencyManager.instance.GetAmount(CurrencyType.Special);
        int reputation = CurrencyManager.instance.GetAmount(CurrencyType.Reputation);

        if (normal != prevNormal)
        {
            prevNormal = normal;
            if (normalCurrencyText != null)
            {
                normalCurrencyText.text = $"일반 재화 : {CurrencyFormatter.Format(normal)}";
            }
        }

        if (special != prevSpecial)
        {
            prevSpecial = special;
            if (specialCurrencyText != null)
            {
                specialCurrencyText.text = $"특수 재화 : {CurrencyFormatter.Format(special)}";
            }
        }

        if (reputation != prevReputation)
        {
            prevReputation = reputation;
            if (reputationText != null)
            {
                reputationText.text = $"명성 : {CurrencyFormatter.Format(reputation)}";
            }
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

                // 등급에 맞는 고용 한도 설정
                RankManager.instance.maxEmployee = RankManager.instance.currentRank switch
                {
                    RankManager.RankState.Indie => 5,
                    RankManager.RankState.Small => 10,
                    RankManager.RankState.Midsized => 20,
                    RankManager.RankState.MajorPublisher => 50,
                    _ => 0
                };
                RankManager.instance.hasEmployees = true;

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
            RankManager.instance.maxEmployee = 50;
            RankManager.instance.hasEmployees = true;
        }

        Debug.Log("[디버그] 모든 조건 프리패스 지급 완료! (일반 1000만, 특수 10000, 명성 3000, 환생 5회, 대기업 등급)");
    }
#endif
}