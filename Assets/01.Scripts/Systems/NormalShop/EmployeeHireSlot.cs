using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public void SetUp(EmployeeState _employeeState, EmployeeManager _employeeManager, Action onHireSuccessCallback = null)
    {
        targetState = _employeeState;
        employeeManager = _employeeManager;
        onHireSuccess = onHireSuccessCallback;

        if (hireBtn == null)
        {
            hireBtn = GetComponent<Button>();
        }

        if(hireBtn != null)
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
                infoText.text = $"[잠김] {data.UnlockGrade}";
            }
            if (costText != null)
            {
                costText.text = "-";
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
            costText.text = isMaxCapacity ? "MAX" : CurrencyFormatter.Format(targetState.GetCurrentHireCost());
        }

        // 버튼 활성화 여부 판정
        if (hireBtn != null)
        {
            hireBtn.interactable = CanHire();
        }

        // 해고 버튼 : 보유 인원이 1명 이상일 때만 활성화
        if (fireBtn != null)
        {
            fireBtn.interactable = targetState.Count > 0;
        }
    }

    public void OnClickHire()
    {
        if (!CanHire() || employeeManager == null)
        {
            return;
        }
        employeeManager.HireEmployee(targetState.employeeData.EmployeeId);
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
        if (targetState == null || targetState.Count <= 0) return;

        var manager = employeeManager != null ? employeeManager : EmployeeManager.instance;
        if (manager == null) return;

        // 직원 해고 실행
        bool success = manager.FireEmployee(targetState.employeeData.EmployeeId);
        if (success)
        {
            // 일괄 갱신
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
}
