using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EmployeeHireSlot : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI infoText; // 직군 이름 및 현재 고용 인원수
    [SerializeField] private TextMeshProUGUI costText; // 고용 비용
    [SerializeField] private Button hireBtn;           // 고용 버튼
    [SerializeField] private Button fireBtn;           // 해고 버튼

    private EmployeeState targetState;
    private EmployeeManager employeeManager;
    private Action onHireSuccess;
    private Color originCostColor = Color.white;

    private void Awake()
    {
        if (costText != null)
        {
            originCostColor = costText.color;
        }
    }

    public void SetUp(EmployeeState _employeeState, EmployeeManager _employeeManager, Action onHireSuccessCallback = null)
    {
        targetState = _employeeState;
        employeeManager = _employeeManager;
        onHireSuccess = onHireSuccessCallback;

        if (hireBtn == null)
        {
            hireBtn = GetComponent<Button>();
        }

        if (hireBtn != null)
        {
            hireBtn.onClick.RemoveListener(OnClickHire);
            hireBtn.onClick.AddListener(OnClickHire);
        }

        if (fireBtn != null)
        {
            fireBtn.onClick.RemoveListener(OnClickFire);
            fireBtn.onClick.AddListener(OnClickFire);
        }

        Refresh();
    }


    private bool CanHire()
    {
        // 필수 데이터 null 체크
        if (targetState?.employeeData == null)
        {
            return false;
        }
        if (RankManager.instance == null || CurrencyManager.instance == null)
        {
            return false;
        }

        // 해금 조건 및 고용 한도 확인
        if (RankManager.instance.currentRank < targetState.employeeData.UnlockGrade)
        {
            return false;
        }
        if (RankManager.instance.currentEmployeeCount >= RankManager.instance.maxEmployee)
        {
            return false;
        }

        // 재화 보유량 확인
        int currentGold = CurrencyManager.instance.GetAmount(CurrencyType.Normal);
        return currentGold >= targetState.GetCurrentHireCost();
    }

    public void Refresh()
    {
        if (targetState?.employeeData == null)
        {
            return;
        }

        var data = targetState.employeeData;
        RankManager rank = RankManager.instance;
        bool isUnlocked = rank != null && rank.currentRank >= data.UnlockGrade;

        if (!isUnlocked)
        {
            if (infoText != null)
            {
                // 직군 이름과 함께 필요 등급을 명시
                infoText.text = $"{data.EmployeeName} ([해금] {GetRankKorean(data.UnlockGrade)})";
            }
            if (costText != null)
            {
                costText.text = "잠김";
                costText.color = new Color(0.6f, 0.6f, 0.6f, 1f);
            }
            if (hireBtn != null)
            {
                hireBtn.interactable = false;
            }
            if (fireBtn != null)
            {
                fireBtn.interactable = false;
            }
            return;
        }

        // 직군명 x인원수 (+초당생산량)
        int totalProduction = data.ProductionPerSecond * targetState.Count;
        if (infoText != null)
        {
            infoText.text = $"{data.EmployeeName} x{targetState.Count} (+{CurrencyFormatter.Format(totalProduction)}/초)";
        }

        // 비용 / MAX 표시
        bool isMaxCapacity = RankManager.instance != null && RankManager.instance.currentEmployeeCount >= RankManager.instance.maxEmployee;
        if (costText != null)
        {
            costText.DOComplete();

            if (isMaxCapacity)
            {
                costText.text = "MAX";
                costText.color = new Color(0.65f, 0.65f, 0.65f, 1f); // 정원 초과 시 회색
            }
            else
            {
                int hireCost = targetState.GetCurrentHireCost();
                costText.text = CurrencyFormatter.Format(hireCost);

                int currentGold = CurrencyManager.instance != null ? CurrencyManager.instance.GetAmount(CurrencyType.Normal) : 0;
                bool canAfford = currentGold >= hireCost;

                // 고용 가능하면 흰색, 돈 부족하면 옅은 회색
                costText.color = canAfford ? originCostColor : new Color(0.65f, 0.65f, 0.65f, 1f);
            }
        }

        // 버튼 활성화 유지
        if (hireBtn != null)
        {
            hireBtn.interactable = true;
        }

        // 해고 버튼 : 보유 인원이 1명 이상일 때만 활성화
        if (fireBtn != null)
        {
            fireBtn.interactable = targetState.Count > 0;
        }
    }

    public void OnClickHire()
    {
        var manager = employeeManager != null ? employeeManager : EmployeeManager.instance;
        if (manager == null)
        {
            return;
        }

        // 고용 조건 미달 시 실패 연출
        if (!CanHire())
        {
            PlayHireFailAnimation();
            return;
        }

        manager.HireEmployee(targetState.employeeData.EmployeeId);

        // 고용 성공 연출
        transform.DOComplete();
        transform.DOPunchScale(Vector3.one * 0.06f, 0.15f, vibrato: 5, elasticity: 0.5f)
                 .SetLink(gameObject);

        if (onHireSuccess != null)
        {
            onHireSuccess.Invoke();
        }
        else
        {
            Refresh();
        }
    }

    public void OnClickFire()
    {
        if (targetState == null || targetState.Count <= 0)
        {
            return;
        }

        var manager = employeeManager != null ? employeeManager : EmployeeManager.instance;
        if (manager == null)
        {
            return;
        }

        bool success = manager.FireEmployee(targetState.employeeData.EmployeeId);
        if (success)
        {
            if (onHireSuccess != null)
            {
                onHireSuccess.Invoke();
            }
            else
            {
                Refresh();
            }
        }
    }

    private void PlayHireFailAnimation()
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