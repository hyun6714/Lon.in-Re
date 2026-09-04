using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopArtifactSlot : MonoBehaviour
{
    [Header("ID")]
    public int targetArtifactID;

    [Header("UI Reference")]
    public Image iconImage;
    public TextMeshProUGUI nameText;       // 아티팩트 이름
    public TextMeshProUGUI costText;       // 비용 (해금 시 '보유 중'으로 변경)
    public TextMeshProUGUI descText;       // 효과 설명 텍스트
    public TextMeshProUGUI conditionText;  // 해금 조건
    public Button unlockBtn;               // 슬롯 본체 버튼

    private ArtifactInfo targetInfo;

    private Action onPurchaseSuccess;      // 상점의 일괄 갱신 함수를 기억해 둘 콜백 변수
    private Action<string, string, Action> onRequestConfirm;    // 팝업 출력 요청할 콜백

    private Color32 normalColor = new Color32(247, 244, 235, 255);
    // private Color32 impossibleColor = new Color32(235, 85, 85, 255);

    public void SetUp(
            ArtifactInfo info,
            Action onPurchaseSuccessCallback = null,
            Action<string, string, Action> onRequestConfirmCallback = null)
    {
        if (info == null)
        {
            return;
        }

        targetInfo = info;
        targetArtifactID = info.artifactId;

        // 전달받은 콜백 함수 저장
        onPurchaseSuccess = onPurchaseSuccessCallback;
        onRequestConfirm = onRequestConfirmCallback;

        // 이름 및 아이콘 설정
        if (iconImage != null && info.icon != null)
        {
            iconImage.sprite = info.icon;
        }
        if (nameText != null)
        {
            nameText.text = info.artiName;
        }

        // 효과 설명 텍스트 동적 구현
        if (descText != null)
        {
            string descStr = "";
            if (info.GainperClick > 0)
            {
                descStr += $"탭 골드 +{info.GainperClick * 100}%  ";
            }
            if (info.PerSecond > 0)
            {
                descStr += $"초당 생산 +{CurrencyFormatter.Format(info.PerSecond)}  ";
            }
            if (info.Probabilityincrease > 0)
            {
                descStr += $"개발 성공률 +{info.Probabilityincrease}%";
            }

            descText.text = descStr.TrimEnd();
            descText.gameObject.SetActive(!string.IsNullOrEmpty(descStr));
        }

        // 조건 텍스트 설정
        string conditionStr = "";
        if (info.requiredRebirthCount > 0)
        {
            conditionStr += $"환생 {info.requiredRebirthCount}회 이상 ";
        }
        if (info.requiredReputation > 0)
        {
            conditionStr += $"명성 {info.requiredReputation} 이상";
        }

        if (conditionText != null)
        {
            conditionText.text = conditionStr;
            conditionText.gameObject.SetActive(!string.IsNullOrEmpty(conditionStr) && !info.isUnlocked);
        }

        // 버튼 리스너 연결
        if (unlockBtn == null)
        {
            unlockBtn = GetComponent<Button>();
        }
        if (unlockBtn != null)
        {
            unlockBtn.onClick.RemoveAllListeners();
            unlockBtn.onClick.AddListener(OnClickUnlock);
        }

        UpdateUIState(info);
    }

    public void UpdateUIState(ArtifactInfo info)
    {
        if (info == null)
        {
            return;
        }

        if (info.isUnlocked)
        {
            // 이미 보유한 아티팩트
            if (unlockBtn != null)
            {
                unlockBtn.interactable = false;
            }
            if (costText != null)
            {
                costText.text = "보유 중";
                costText.color = normalColor;
            }
            if (conditionText != null)
            {
                conditionText.gameObject.SetActive(false);
            }
        }
        else
        {
            // 미보유 아티팩트
            if (costText != null)
            {
                costText.text = CurrencyFormatter.Format(info.SpecialUnlockCost);
                costText.color = normalColor;
            }

            // 3가지 조건 검사 후 버튼 활성/비활성화
            int currentSpecial = CurrencyManager.instance != null ? CurrencyManager.instance.GetAmount(CurrencyType.Special) : 0;
            int currentReputation = CurrencyManager.instance != null ? CurrencyManager.instance.GetAmount(CurrencyType.Reputation) : 0;
            int currentRebirth = GameManager.Instance != null ? GameManager.Instance.playerRebirthCount : 0;

            bool canPurchase = (currentSpecial >= info.SpecialUnlockCost) &&
                               (currentRebirth >= info.requiredRebirthCount) &&
                               (currentReputation >= info.requiredReputation);

            if (unlockBtn != null)
            {
                unlockBtn.interactable = canPurchase;
            }
        }
    }

    public void Refresh()
    {
        if (targetInfo != null)
        {
            UpdateUIState(targetInfo);
        }
    }

    // 슬롯 터치 시 실행되는 함수
    public void OnClickUnlock()
    {
        // 이미 보유 중이거나 데이터가 없으면 무시
        if (targetInfo == null || targetInfo.isUnlocked)
        {
            return;
        }
        if (CurrencyManager.instance == null || GameManager.Instance == null)
        {
            return;
        }

        // 현재 값 가져오기
        int currentSpecialCurrency = CurrencyManager.instance.GetAmount(CurrencyType.Special);
        int currentReputation = CurrencyManager.instance.GetAmount(CurrencyType.Reputation);
        int currentReincarnation = GameManager.Instance.playerRebirthCount;

        // 해금 조건 검사
        if (currentReincarnation < targetInfo.requiredRebirthCount)
        {
            Debug.LogWarning($"[구매 불가] 환생 횟수가 부족합니다. (필요: {targetInfo.requiredRebirthCount}회 / 현재: {currentReincarnation}회)");
            return;
        }

        if (currentReputation < targetInfo.requiredReputation)
        {
            Debug.LogWarning($"[구매 불가] 명성이 부족합니다. (필요: {targetInfo.requiredReputation} / 현재: {currentReputation})");
            return;
        }

        if (currentSpecialCurrency < targetInfo.SpecialUnlockCost)
        {
            Debug.LogWarning($"[구매 불가] 특수 재화가 부족합니다. (필요: {targetInfo.SpecialUnlockCost}개 / 현재: {currentSpecialCurrency}개)");
            return;
        }

        if (onRequestConfirm != null)
        {
            onRequestConfirm.Invoke(
                "구매 확인",
                $"[{targetInfo.artiName}]\n정말 구매하시겠습니까?",
                ExecuteUnlock
            );
        }
        else
        {
            ExecuteUnlock();
        }
    }

    // 실제 구매 함수
    private void ExecuteUnlock()
    {
        if (CurrencyManager.instance == null || GameManager.Instance == null || ArtifactManager.instance == null)
        {
            return;
        }

        int currentSpecialCurrency = CurrencyManager.instance.GetAmount(CurrencyType.Special);
        int currentReputation = CurrencyManager.instance.GetAmount(CurrencyType.Reputation);
        int currentReincarnation = GameManager.Instance.playerRebirthCount;

        ArtifactInfo info = ArtifactManager.instance.artifactDatabase.GetArtifactsInfo(targetArtifactID);

        // 실제 구매 및 해금 검사
        bool success = ArtifactManager.instance.TryUnlockArtifact(
            targetArtifactID,
            currentReincarnation,
            currentReputation,
            currentSpecialCurrency
        );

        if (success)
        {
            Debug.Log($"해금 완료: {targetInfo.artiName}");
            if (onPurchaseSuccess != null)
            {
                onPurchaseSuccess.Invoke();
            }
            else if (info != null)
            {
                UpdateUIState(info);
            }
        }
    }
}