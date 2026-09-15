using UnityEngine;

[CreateAssetMenu(fileName = "GameReleaseData", menuName = "Game/Game Release Data")]
public class GameReleaseData : ScriptableObject
{
    [Header("A등급 보상")]
    [SerializeField] private int gradeACurrencyReward;
    [SerializeField] private int gradeAReputationReward;

    [Header("B등급 보상")]
    [SerializeField] private int gradeBCurrencyReward;
    [SerializeField] private int gradeBReputationReward;

    [Header("C등급 보상")]
    [SerializeField] private int gradeCCurrencyReward;
    [SerializeField] private int gradeCReputationReward;

    public int GradeACurrencyReward => gradeACurrencyReward;
    public int GradeAReputationReward => gradeAReputationReward;

    public int GradeBCurrencyReward => gradeBCurrencyReward;
    public int GradeBReputationReward => gradeBReputationReward;

    public int GradeCCurrencyReward => gradeCCurrencyReward;
    public int GradeCReputationReward => gradeCReputationReward;

    public int GetCurrencyReward(DevelopmentGrade grade)
    {
        switch (grade)
        {
            case DevelopmentGrade.A:
                return gradeACurrencyReward;

            case DevelopmentGrade.B:
                return gradeBCurrencyReward;

            case DevelopmentGrade.C:
                return gradeCCurrencyReward;
        }

        return 0;
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