using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeHireSlot : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI infoText; // 직군 이름 및 현재 고용 인원수
    [SerializeField] private TextMeshProUGUI costText; // 고용 비용
    [SerializeField] private Button hireBtn;           // 슬롯 버튼

    private EmployeeState targetState;
    private EmployeeManager employeeManager;

    public void SetUp(EmployeeState _employeeState, EmployeeManager _employeeManager)
    {
        targetState = _employeeState;
        employeeManager = _employeeManager;

        if(hireBtn == null)
        {
            hireBtn = GetComponent<Button>();
        }

        if(hireBtn != null)
        {
            hireBtn.onClick.RemoveAllListeners();
            hireBtn.onClick.AddListener(OnClickHire);
        }
        Refresh();
    }

    private void Update()
    {
        if (hireBtn != null)
        {
            hireBtn.interactable = CanHire();
        }
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

        // 해금 여부 확인
        bool isUnlocked = RankManager.instance != null && RankManager.instance.currentRank >= data.UnlockGrade;
        if (!isUnlocked)
        {
            if (infoText != null) infoText.text = $"[잠김] {data.UnlockGrade}";
            if (costText != null) costText.text = "-";
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
    }

    public void OnClickHire()
    {
        if (!CanHire() || employeeManager == null)
        {
            return;
        }
        employeeManager.HireEmployee(targetState.employeeData.EmployeeId);
        Refresh();
    }
}
