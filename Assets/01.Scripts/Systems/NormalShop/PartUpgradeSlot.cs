using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartUpgradeSlot : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI infoText; // Item Name 텍스트 (이름 + Lv + 효과)
    [SerializeField] private TextMeshProUGUI costText; // 0 텍스트 (비용 숫자)
    [SerializeField] private Button slotBtn;           // 슬롯 버튼

    private PartState targetState;
    private PlayerTapUpgrade playerUpgrade;
    private Action onUpgradeSuccess;                   // 모든 부품 슬롯 일괄 갱신용 콜백

    public void SetUp(PartState _targetState, PlayerTapUpgrade _playerUpgrade, Action onUpgradeSuccessCallback = null)
    {
        targetState = _targetState;
        playerUpgrade = _playerUpgrade;
        onUpgradeSuccess = onUpgradeSuccessCallback;

        if (slotBtn == null)
        {
            slotBtn = GetComponent<Button>();
        }

        if (slotBtn != null)
        {
            slotBtn.onClick.RemoveListener(OnClickUpgrade);
            slotBtn.onClick.AddListener(OnClickUpgrade);
        }

        Refresh();
    }

    public void Refresh()
    {
        if (targetState == null || targetState.partData == null)
        {
            return;
        }

        var data = targetState.partData;

        if (iconImage != null && data.Icon != null)
        {
            iconImage.sprite = data.Icon;
        }

        // 해금 여부 확인
        bool isUnlocked = RankManager.instance != null && RankManager.instance.currentRank >= data.UnlockGrade;

        if (!isUnlocked)
        {
            if (infoText != null) infoText.text = $"[잠김] {data.UnlockGrade}";
            if (costText != null) costText.text = "-";
            if (slotBtn != null) slotBtn.interactable = false;
            return;
        }

        // 이름 Lv.N (+수치)
        if (infoText != null)
        {
            infoText.text = $"{data.PartName} Lv.{targetState.Level} (+{CurrencyFormatter.Format(data.PowerPerLevel)})";
        }

        // 비용
        if (costText != null)
        {
            costText.text = CurrencyFormatter.Format(targetState.GetNextCost());
        }

        // 버튼 활성화 여부 갱신
        if (slotBtn != null && playerUpgrade != null)
        {
            slotBtn.interactable = playerUpgrade.CanUpgrade(targetState);
        }
    }

    public void OnClickUpgrade()
    {
        if (playerUpgrade == null || targetState == null)
        {
            return;
        }

        if (playerUpgrade.TryUpgrade(targetState))
        {
            if (onUpgradeSuccess != null)
            {
                onUpgradeSuccess.Invoke();
            }
            else
            {
                Refresh();
            }
        }
    }
}
