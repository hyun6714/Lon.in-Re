using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArtifactSlot : MonoBehaviour
{
    //프리팹 만들기 UI에 적용할 
    [Header("ID")] 
    public int targetArtifactID;

    [Header("UI Reference")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI conditionText;//해금조건
    public Button unlockBtn;

    public void SetUp(ArtifactInfo info)
    {
        if (info != null)
        {
            targetArtifactID = info.artifactId;
        }

        //기본 세팅
        if (iconImage != null) iconImage.sprite = info.icon;
        if (nameText != null) nameText.text = info.artiName;
        if (costText != null) costText.text = $"특수재화 : {info.SpecialUnlockCost}";

        string conditionStr = "";
        if (info.requiredRebirthCount > 0) conditionStr += $"환생 {info.requiredRebirthCount}회 이상 ";
        if (info.requiredReputation > 0) conditionStr += $" 명성 {info.requiredReputation} 이상";

        if (conditionText != null)
        {
            conditionText.text = conditionStr;
            conditionText.gameObject.SetActive(!string.IsNullOrEmpty(conditionStr)); //문자열이 비어있으면 끄고 아니면 키기
        }

        UpdateUIState(info);
    }

    public void UpdateUIState(ArtifactInfo info)
    {
        if (info.isUnlocked)
        {
            unlockBtn.interactable = false;
            var buttonText = unlockBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "보유 중";
            }
        }
        else
        {
            unlockBtn.interactable = true;
            var buttonText = unlockBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "구매 하기";
            }
        }
    }

    public void OnCilckUnlock()
    {
        int currentSpecialCurrency = CurrencyManager.instance.GetAmount(CurrencyType.Special); // 현재 특수재화 가져오기
        int currentReputation = CurrencyManager.instance.GetAmount(CurrencyType.Reputation); // 현재 명성 가져오기
        int currentReincarnation = GameManager.Instance.playerRebirthCount; // 현재 환생 횟수

        ArtifactInfo info = ArtifactManager.instance.artifactDatabase.GetArtifactsInfo(targetArtifactID);

        bool success = ArtifactManager.instance.TryUnlockArtifact (targetArtifactID, currentReincarnation, currentReputation, currentSpecialCurrency);
 
        if (success)
        {
            Debug.Log($"해금 완료");

            if (info != null)
            {
                UpdateUIState(info);
            }
        }
    }
}
