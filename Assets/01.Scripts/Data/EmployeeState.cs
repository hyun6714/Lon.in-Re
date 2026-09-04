using System;
using UnityEngine;

// 현재 플레이어가 보유한 직원 상태 
[Serializable]
public class EmployeeState
{
    // 직원 데이터
    public EmployeeData employeeData;

    // 현재 보유 인원
    [SerializeField] private int count;
    public int Count => count;

    // 고용상태에 따른 다음 고용비 계산
    // 2번째 고용부터 적용
    public int GetCurrentHireCost()
    {
        double cost = employeeData.BaseHireCost * Math.Pow(employeeData.HireCostMultiplier, count);

        return (int)Math.Round(cost);
    }

    // 직원 고용 시 보유 인원 증가
    public void AddEmployee()
    {
        count++;
    }
}

