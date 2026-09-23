using UnityEngine;
using System;

[CreateAssetMenu(fileName = "GameReleaseData", menuName = "Game/Game Release Data")]
public class GameReleaseData : ScriptableObject
{
    [Header("등급별 재화 보상 비율 (%)")]
    [SerializeField] private int gradeACurrencyPercent = 125;
    [SerializeField] private int gradeBCurrencyPercent = 110;
    [SerializeField] private int gradeCCurrencyPercent = 80;

    [Header("Solo 명성 보상")]
    [SerializeField] private int soloAReputationReward = 2000;
    [SerializeField] private int soloBReputationReward = 1000;
    [SerializeField] private int soloCReputationReward = 500;

    [Header("Indie 명성 보상")]
    [SerializeField] private int indieAReputationReward = 5000;
    [SerializeField] private int indieBReputationReward = 3000;
    [SerializeField] private int indieCReputationReward = 2000;

    [Header("Small 명성 보상")]
    [SerializeField] private int smallAReputationReward = 10000;
    [SerializeField] private int smallBReputationReward = 7000;
    [SerializeField] private int smallCReputationReward = 5000;

    [Header("Midsized 명성 보상")]
    [SerializeField] private int midsizedAReputationReward = 30000;
    [SerializeField] private int midsizedBReputationReward = 15000;
    [SerializeField] private int midsizedCReputationReward = 10000;

    [Header("MajorPublisher 명성 보상")]
    [SerializeField] private int majorAReputationReward = 70000;
    [SerializeField] private int majorBReputationReward = 50000;
    [SerializeField] private int majorCReputationReward = 30000;

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

    public int GetReputationReward(
        RankManager.RankState rank,
        DevelopmentGrade grade
    )
    {
        switch (rank)
        {
            case RankManager.RankState.Solo:
                switch (grade)
                {
                    case DevelopmentGrade.A:
                        return soloAReputationReward;
                    case DevelopmentGrade.B:
                        return soloBReputationReward;
                    case DevelopmentGrade.C:
                        return soloCReputationReward;
                }
                break;

            case RankManager.RankState.Indie:
                switch (grade)
                {
                    case DevelopmentGrade.A:
                        return indieAReputationReward;
                    case DevelopmentGrade.B:
                        return indieBReputationReward;
                    case DevelopmentGrade.C:
                        return indieCReputationReward;
                }
                break;

            case RankManager.RankState.Small:
                switch (grade)
                {
                    case DevelopmentGrade.A:
                        return smallAReputationReward;
                    case DevelopmentGrade.B:
                        return smallBReputationReward;
                    case DevelopmentGrade.C:
                        return smallCReputationReward;
                }
                break;

            case RankManager.RankState.Midsized:
                switch (grade)
                {
                    case DevelopmentGrade.A:
                        return midsizedAReputationReward;
                    case DevelopmentGrade.B:
                        return midsizedBReputationReward;
                    case DevelopmentGrade.C:
                        return midsizedCReputationReward;
                }
                break;

            case RankManager.RankState.MajorPublisher:
                switch (grade)
                {
                    case DevelopmentGrade.A:
                        return majorAReputationReward;
                    case DevelopmentGrade.B:
                        return majorBReputationReward;
                    case DevelopmentGrade.C:
                        return majorCReputationReward;
                }
                break;
        }

        return 0;
    }
}