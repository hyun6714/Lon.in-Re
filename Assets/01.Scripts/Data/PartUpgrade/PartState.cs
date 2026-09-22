using System;
using UnityEngine;

[Serializable]
public class PartState
{
    public PartData partData;
    [SerializeField] private int level = 0;

    public int Level => level;

    // 현재 부품이 제공하는 총 탭 파워
    public int GetTotalPower()
    {
        double power = partData.BasePower * Math.Pow(partData.PowerMultiplier, level);
        return (int)Math.Round(power);
    }

    // 다음 레벨 강화 비용 계산 : BaseCost * (Multiplier ^ Level)
    public int GetNextCost()
    {
        double cost = partData.BaseCost * Math.Pow(partData.CostMultiplier, level);
        return (int)Math.Round(cost);
    }

    public int GetCurrentPower()
    {
        double power = partData.BasePower * Math.Pow(partData.PowerMultiplier, level);
        return (int)Math.Round(power);
    }

    public int GetNextPower()
    {
        double power = partData.BasePower * Math.Pow(partData.PowerMultiplier, level + 1);
        return (int)Math.Round(power);
    }

    public void LevelUp()
    {
        level++;
    }

    public void SetLevel(int value)
    {
        level = Mathf.Max(0, value);
    }

    public void ResetLevel()
    {
        level = 0;
    }
}