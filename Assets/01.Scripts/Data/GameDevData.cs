using UnityEngine;


[CreateAssetMenu(fileName = "GameDevelopmentData", menuName = "Game/Game Development Data")]

public class GameDevData : ScriptableObject
{
    [Header("개발 비용")]
    [SerializeField] private int developmentCost = 10000;

    [Header("확률 추가 비용")]
    [SerializeField] private int probabilityUpgradeCost = 1000;

    [Header("최대 확률 추가 횟수")]
    [SerializeField] private int maxProbabilityUpgradeCount = 3;

    [Header("기본 등급 확률")] //나중에 수치 조정 가능
    [SerializeField] private float gradeARate = 33f;
    [SerializeField] private float gradeBRate = 33f;
    [SerializeField] private float gradeCRate = 34f;

    [Header("확률 추가 1회당 등급 변화")]
    [SerializeField] private float gradeAIncreaseRate = 3f;
    [SerializeField] private float gradeBIncreaseRate = 2f;
    [SerializeField] private float gradeCDecreaseRate = 5f;

    [Header("최종 등급 점수 기준")] // 나중에 수정 가능
    [SerializeField] private int finalAGradeScore = 7;
    [SerializeField] private int finalBGradeScore = 5;

    public int DevelopmentCost => developmentCost;

    public int ProbabilityUpgradeCost => probabilityUpgradeCost;

    public int MaxProbabilityUpgradeCount => maxProbabilityUpgradeCount;

    public float GradeARate => gradeARate;
    public float GradeBRate => gradeBRate;
    public float GradeCRate => gradeCRate;

    public float GradeAIncreaseRate => gradeAIncreaseRate;
    public float GradeBIncreaseRate => gradeBIncreaseRate;
    public float GradeCDecreaseRate => gradeCDecreaseRate;

    public int FinalAGradeScore => finalAGradeScore;
    public int FinalBGradeScore => finalBGradeScore;
}
