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
        return level * partData.PowerPerLevel;
    }

    // 다음 레벨 강화 비용 계산 : BaseCost * (Multiplier ^ Level)
    public int GetNextCost()
    {
        double cost = partData.BaseCost * Math.Pow(partData.CostMultiplier, level);
        return (int)Math.Round(cost);
    }

    public void LevelUp()
    {
        level++;
    }

    public void ResetLevel()
    {
        level = 0;
    }
}