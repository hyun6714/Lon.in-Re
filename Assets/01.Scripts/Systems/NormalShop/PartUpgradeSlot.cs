using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PartUpgradeSlot : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI infoText; // Item Name 텍스트 (이름 + Lv + 효과)
    [SerializeField] private TextMeshProUGUI costText; // 0 텍스트 (비용 숫자)
    [SerializeField] private Button slotBtn;           // 슬롯 버튼

    private PartState targetState;
    private PlayerTapUpgrade playerUpgrade;
    private Action onUpgradeSuccess;                    // 모든 부품 슬롯 일괄 갱신용 콜백
    private Color originCostColor = Color.white;

    private void Awake()
    {
        if (costText != null)
        {
            originCostColor = costText.color;
        }
    }

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
            if (infoText != null) infoText.text = $"{data.PartName} ([해금] {GetRankKorean(data.UnlockGrade)})";
            if (costText != null) costText.text = "잠김";
            if (slotBtn != null) slotBtn.interactable = false;
            return;
        }

        // 이름 Lv.N (+기본 수치)
        infoText.text = $"{data.PartName} Lv.{targetState.Level} (+{CurrencyFormatter.Format(targetState.GetCurrentPower())})";

        // 비용
        if (costText != null)
        {
            long nextCost = targetState.GetNextCost();
            costText.text = CurrencyFormatter.Format(nextCost);

            CheckCost();
        }

        // 버튼 활성화 유지
        if (slotBtn != null)
        {
            slotBtn.interactable = true;
        }
    }

    public void CheckCost()
    {
        if (targetState == null || targetState.partData == null || costText == null)
        {
            return;
        }

        bool isUnlocked = RankManager.instance != null && RankManager.instance.currentRank >= targetState.partData.UnlockGrade;
        if (!isUnlocked)
        {
            return;
        }

        long nextCost = targetState.GetNextCost();
        bool canBuy = CurrencyManager.instance != null && CurrencyManager.instance.GetAmount(CurrencyType.Normal) >= nextCost;

        costText.color = canBuy ? originCostColor : new Color(0.65f, 0.65f, 0.65f, 1f);
    }

    public void OnClickUpgrade()
    {
        if (playerUpgrade == null || targetState == null)
        {
            return;
        }

        // 업그레이드 시도
        if (playerUpgrade.TryUpgrade(targetState))
        {
            // 업그레이드 성공 연출
            transform.DOComplete();
            transform.DOPunchScale(Vector3.one * 0.06f, 0.15f, vibrato: 5, elasticity: 0.5f)
                     .SetLink(gameObject);

            Refresh();

            onUpgradeSuccess?.Invoke();
        }
        else
        {
            // 업그레이드 실패
            PlayFailAnimation();
        }
    }

    private void PlayFailAnimation()
    {
        // 슬롯 좌우 진동
        transform.DOComplete();
        transform.DOShakePosition(0.25f, strength: new Vector3(8f, 0, 0), vibrato: 10, randomness: 90, fadeOut: true)
                 .SetLink(gameObject);
    }

    private string GetRankKorean(RankManager.RankState rank)
    {
        switch (rank)
        {
            case RankManager.RankState.Solo:
                return "1인 개발";
            case RankManager.RankState.Indie:
                return "인디 기업";
            case RankManager.RankState.Small:
                return "중소기업";
            case RankManager.RankState.Midsized:
                return "중견기업";
            case RankManager.RankState.MajorPublisher:
                return "대기업";
            default:
                return "잠김";
        }
    }
}