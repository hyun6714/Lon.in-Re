using UnityEngine;
using System;

[CreateAssetMenu(fileName = "GameReleaseData", menuName = "Game/Game Release Data")]
public class GameReleaseData : ScriptableObject
{
    [Header("등급별 재화 보상 비율 (%)")]
    [SerializeField] private int gradeACurrencyPercent = 700;
    [SerializeField] private int gradeBCurrencyPercent = 300;
    [SerializeField] private int gradeCCurrencyPercent = 110;

    [Header("A등급 명성 보상")]
    [SerializeField] private int gradeAReputationReward = 800;

    [Header("B등급 명성 보상")]
    [SerializeField] private int gradeBReputationReward = 400;

    [Header("C등급 명성 보상")]
    [SerializeField] private int gradeCReputationReward = 250;

    public int GetCurrencyReward(DevelopmentGrade grade, int baseDevelopmentCost)
    {
        int rewardPercent = 0;

        switch (grade)
        {
            case DevelopmentGrade.A:
                rewardPercent = gradeACurrencyPercent;
                break;

            case DevelopmentGrade.B:
                rewardPercent = gradeBCurrencyPercent;
                break;

            case DevelopmentGrade.C:
                rewardPercent = gradeCCurrencyPercent;
                break;
        }

        long reward =
            (long)baseDevelopmentCost * rewardPercent / 100;

        return (int)Math.Min(reward, int.MaxValue);
    }

    public int GetReputationReward(DevelopmentGrade grade)
    {
        switch (grade)
        {
            case DevelopmentGrade.A:
                return gradeAReputationReward;

            case DevelopmentGrade.B:
                return gradeBReputationReward;

            case DevelopmentGrade.C:
                return gradeCReputationReward;
        }

        return 0;
    }
}