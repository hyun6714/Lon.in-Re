using UnityEngine;


[CreateAssetMenu(fileName = "GameDevelopmentData", menuName = "Game/Game Development Data")]

public class GameDevData : ScriptableObject
{
    [Header("랭크별 개발 비용")]
    [SerializeField] private int soloDevelopmentCost = 50000;
    [SerializeField] private int indieDevelopmentCost = 300000;
    [SerializeField] private int smallDevelopmentCost = 2000000;
    [SerializeField] private int midsizedDevelopmentCost = 30000000;
    [SerializeField] private int majorPublisherDevelopmentCost = 100000000;

    [Header("확률 추가 비용 비율")]
    [SerializeField] private float probabilityUpgradeCostRate = 0.05f;

    [Header("최대 확률 추가 횟수")]
    [SerializeField] private int maxProbabilityUpgradeCount = 3;

    [Header("기본 등급 확률")] //나중에 수치 조정 가능
    [SerializeField] private float gradeARate = 15f;
    [SerializeField] private float gradeBRate = 30f;
    [SerializeField] private float gradeCRate = 55f;

    [Header("확률 추가 1회당 등급 변화")]
    [SerializeField] private float gradeAIncreaseRate = 8f;
    [SerializeField] private float gradeBIncreaseRate = 2f;
    [SerializeField] private float gradeCDecreaseRate = 10f;

    [Header("최종 등급 점수 기준")] // 나중에 수정 가능
    [SerializeField] private int finalAGradeScore = 8;
    [SerializeField] private int finalBGradeScore = 5;

    [Header("최대 동시 개발 게임 수")]
    [SerializeField] private int maxActiveGameCount = 10;

    public float ProbabilityUpgradeCostRate => probabilityUpgradeCostRate;

    public int GetDevelopmentCost(RankManager.RankState rank)
    {
        switch (rank)
        {
            case RankManager.RankState.Solo:
                return soloDevelopmentCost;

            case RankManager.RankState.Indie:
                return indieDevelopmentCost;

            case RankManager.RankState.Small:
                return smallDevelopmentCost;

            case RankManager.RankState.Midsized:
                return midsizedDevelopmentCost;

            case RankManager.RankState.MajorPublisher:
                return majorPublisherDevelopmentCost;
        }

        return soloDevelopmentCost;
    }

    public int MaxProbabilityUpgradeCount => maxProbabilityUpgradeCount;

    public float GradeARate => gradeARate;
    public float GradeBRate => gradeBRate;
    public float GradeCRate => gradeCRate;

    public float GradeAIncreaseRate => gradeAIncreaseRate;
    public float GradeBIncreaseRate => gradeBIncreaseRate;
    public float GradeCDecreaseRate => gradeCDecreaseRate;

    public int FinalAGradeScore => finalAGradeScore;
    public int FinalBGradeScore => finalBGradeScore;

    public int MaxActiveGameCount => maxActiveGameCount;
}
