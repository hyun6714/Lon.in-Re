using UnityEngine;

[CreateAssetMenu(fileName = "EmployeeData", menuName = "Game/Employee Data")]
public class EmployeeData : ScriptableObject
{
    [Header("직원 기본 정보")]

    // 직원 고유 ID
    [SerializeField] private string employeeId;

    // 직원 이름
    [SerializeField] private string employeeName;


    [Header("해금 조건")]

    // 회사 등급
   [SerializeField] private RankManager.RankState unlockGrade;


    [Header("고용 정보")]

    // 고용 비용
    [SerializeField] private int baseHireCost;

    // 두번째 고용시 n배만큼 가격 증가
    [SerializeField] private float hireCostMultiplier = 1.3f;


    [Header("생산 정보")]

    //초당 골드 생산량
    [SerializeField] private int productionPerSecond;


    public string EmployeeId => employeeId;

    public string EmployeeName => employeeName;

    public RankManager.RankState UnlockGrade => unlockGrade;

    public int BaseHireCost => baseHireCost;

    public float HireCostMultiplier => hireCostMultiplier;

    public int ProductionPerSecond => productionPerSecond;
}
