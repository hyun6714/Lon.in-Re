using System.Collections.Generic;
using UnityEngine;
using System;
public class EmployeeManager : MonoBehaviour
{
    public static EmployeeManager instance { get; private set; }

    [SerializeField] private RankManager rankManager;
    [SerializeField] private GameManager gameManger;

    [Header("직원 상태 목록")]
    [SerializeField] private List<EmployeeState> employeeStates = new List<EmployeeState>();

    public List<EmployeeState> EmployeeStates => employeeStates;

    // 환생 이벤트 구독 / 해제
    private void OnEnable()
    {
        GameEventBridge.OnReincarnated += ResetAllEmployees;
    }

    private void OnDisable()
    {
        GameEventBridge.OnReincarnated -= ResetAllEmployees;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (rankManager == null && RankManager.instance != null)
        {
            rankManager = RankManager.instance;
        }

        if (gameManger == null && GameManager.instance != null)
        {
            gameManger = GameManager.instance;
        }
    }

    // 직원 ID 로 직원 찾기 
    public EmployeeState GetEmployeeState(string employeeId)
    {
        foreach (EmployeeState state in employeeStates)
        {
            if (state.employeeData.EmployeeId == employeeId)
            {
                return state;
            }
        }

        Utils.Log($"직원 ID를 찾을 수 없습니다 : {employeeId}");
        return null;
    }

    public void HireEmployee(string employeeId)
    {
        // 1. 직원 찾기
        EmployeeState state = GetEmployeeState(employeeId);

        if (state == null)
        {
            return;
        }

        // 2. 현재 회사 등급에서 해금된 직원인지 확인
        if (rankManager.currentRank < state.employeeData.UnlockGrade)
        {
            Utils.Log("아직 해금되지 않은 직원입니다.");
            return;
        }

        // 3. 등급별 최대 고용 인원 확인
        if (gameManger.currentEmployeeCount >= rankManager.maxEmployee)
        {
            Utils.Log("최대 고용 인원에 도달했습니다.");
            return;
        }

        // 4. 현재 고용 비용 계산
        int hireCost = state.GetCurrentHireCost();

        // 5. CurrencyManager 존재 확인
        if (CurrencyManager.instance == null)
        {
            Utils.Log("CurrencyManager를 찾을 수 없습니다.");
            return;
        }

        // 6. 일반 재화 차감
        bool success = CurrencyManager.instance.UseCurrency(CurrencyType.Normal, hireCost );

        // 재화가 부족하면 고용 취소
        if (!success)
        {
            return;
        }

        // 7. 해당 직원 보유 수 증가
        state.AddEmployee();

        // 8. 전체 직원 수 증가
        gameManger.currentEmployeeCount++;

        // 9. 직원 고용 후 생산량 갱신 이벤트
        GameEventBridge.EmployeeChanged();
        GameEventBridge.EmployeeCountChanged(gameManger.currentEmployeeCount, rankManager.maxEmployee);

        RankUI.instance.UpdateRankUI();

        Utils.Log($"{state.employeeData.EmployeeName} 고용 완료 / " + $"현재 보유 수 : {state.Count}");
    }

    // 직원 1명 해고
    public bool FireEmployee(string employeeId)
    {
        EmployeeState state = GetEmployeeState(employeeId);
        if (state == null || state.Count <= 0)
        {
            Utils.Log("해고할 직원이 없습니다.");
            return false;
        }

        // 해당 직원 보유 수 감소
        state.RemoveEmployee();

        // 전체 직원 카운트 감소 (최소 0)
        if (rankManager != null)
        {
            gameManger.currentEmployeeCount = Mathf.Max(0, gameManger.currentEmployeeCount - 1);
        }
        else if (GameManager.instance != null)
        {
            GameManager.instance.currentEmployeeCount = Mathf.Max(0, GameManager.instance.currentEmployeeCount - 1);
        }

        // 초당 생산량 갱신 이벤트 발생
        GameEventBridge.EmployeeChanged();
        GameEventBridge.EmployeeCountChanged(gameManger.currentEmployeeCount, rankManager.maxEmployee);

        RankUI.instance.UpdateRankUI();

        Utils.Log($"{state.employeeData.EmployeeName} 해고 완료 / 남은 인원: {state.Count}");
        return true;
    }

    // 현재 보유한 모든 직원의 초당 생산량 계산
    public float GetTotalProductionPerSecond()
    {
        float totalProduction = 0;

        foreach (EmployeeState state in employeeStates)
        {
            if (state == null || state.employeeData == null)
            {
                continue;
            }

            // 직원별 생산량 계산 (추후 AutoProduction 에서 사용가능)
            totalProduction += state.GetCurrentProductionPerSecond();
        }

        return totalProduction;
    }

    // 환생 시 모든 직원 수 리셋
    public void ResetAllEmployees()
    {
        RankUI.instance.UpdateRankUI();
        foreach (var state in employeeStates)
        {
            state?.ResetCount();
        }

        GameEventBridge.EmployeeChanged();
        GameEventBridge.EmployeeCountChanged(gameManger.currentEmployeeCount, rankManager.maxEmployee);
        Utils.Log("모든 직원이 초기화되었습니다.");
    }
}
