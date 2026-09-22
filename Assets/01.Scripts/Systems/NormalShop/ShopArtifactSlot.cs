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

    private void OnEnable()
    {
        Refresh();
    }

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
            foreach (var effect in info.effects)
            {
                switch (effect.effectType)
                {
                    case EffectType.GainPerClick:
                        descStr += $"탭 골드 +{effect.effectValue}배  ";
                        break;
                    case EffectType.PerSecond:
                        descStr += $"초당 생산 +{effect.effectValue}배";
                        break;
                    case EffectType.ProbabilityIncrease:
                        descStr += $"개발 성공률 +{effect.effectValue}% ";
                        break;
                }
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

        bool isUnlocked = ArtifactManager.instance != null && ArtifactManager.instance.IsUnlocked(targetArtifactID);

        if (conditionText != null)
        {
            conditionText.text = conditionStr;
            conditionText.gameObject.SetActive(!string.IsNullOrEmpty(conditionStr) && !isUnlocked);
        }

        // 버튼 리스너 연결
        if (unlockBtn == null)
        {
            unlockBtn = GetComponent<Button>();
        }
        if (unlockBtn != null)
        {
            unlockBtn.onClick.RemoveListener(OnClickUnlock);
            unlockBtn.onClick.AddListener(OnClickUnlock);
        }

        UpdateUIState();
    }

    public void UpdateUIState()
    {
        if (targetInfo == null) return;

        bool isUnlocked = ArtifactManager.instance != null && ArtifactManager.instance.IsUnlocked(targetInfo.artifactId);

        if (isUnlocked)
        {
            if (unlockBtn != null) unlockBtn.interactable = false;
            if (costText != null) costText.text = "보유 중";
        }
        else
        {
            if (costText != null) costText.text = CurrencyFormatter.Format(targetInfo.SpecialUnlockCost);

            // 전달받은 값으로 조건 검사
            long currentSpecial = CurrencyManager.instance != null ? CurrencyManager.instance.GetAmount(CurrencyType.Special) : 0;
            long currentReputation = CurrencyManager.instance != null ? CurrencyManager.instance.GetAmount(CurrencyType.Reputation) : 0;
            long currentRebirth = GameManager.instance != null ? GameManager.instance.playerRebirthCount : 0;

            bool canPurchase = (currentSpecial >= targetInfo.SpecialUnlockCost) &&
                               (currentRebirth >= targetInfo.requiredRebirthCount) &&
                               (currentReputation >= targetInfo.requiredReputation);

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
            UpdateUIState();
        }
    }

    // 슬롯 터치 시 실행되는 함수
    public void OnClickUnlock()
    {
        bool isUnlocked = ArtifactManager.instance != null && ArtifactManager.instance.IsUnlocked(targetArtifactID);

        // 이미 보유 중이거나 데이터가 없으면 무시
        if (targetInfo == null || isUnlocked)
        {
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
        if (ArtifactManager.instance == null) return;

        bool success = ArtifactManager.instance.TryUnlockArtifact(targetArtifactID);

        if (success)
        {
            Debug.Log($"해금 완료: {targetInfo.artiName}");

            if (onPurchaseSuccess != null)
            {
                onPurchaseSuccess.Invoke();
            }
            else if (targetInfo != null)
            {
                UpdateUIState();
            }
        }
    }
}