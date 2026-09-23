using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrencyExchangeUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ExchangeDataSO exchangeData;

    [Header("UI References")]
    [SerializeField] private Button exchangeButton; //실행할 버튼 
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI rewardText;

    private void Start()
    {
        if (exchangeButton != null)
        {
            exchangeButton.onClick.AddListener(OnExchangeButtonClicked);
        }

        UpdateUI();
    }

    private void OnDestroy()
    {
        if (exchangeButton != null)
        {
            exchangeButton.onClick.RemoveListener(OnExchangeButtonClicked);
        }
    }

    private void OnExchangeButtonClicked()
    {
        if (exchangeData == null)
        {
            Debug.LogWarning("ExchangeDataSO가 연결되지 않았습니다.");
            return;
        }

        GameEventBridge.CurrencyExchangeRequested(exchangeData);

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (exchangeData == null) return;

        // 비용 텍스트 설정
        if (costText != null)
        {
            costText.text = exchangeData.CostAmount.ToString("N0");
        }

        // 보상 텍스트 설정
        if (rewardText != null)
        {
            rewardText.text = exchangeData.RewardAmount.ToString("N0");
        }
    }
}