using UnityEngine;

[CreateAssetMenu(fileName = "SummerEventData", menuName = "SeasonEvent/SummerEventData")]
public class SummerEventData : ScriptableObject
{
    [Header("기본 생산 배수")]
    [SerializeField] private float autoMultiplier = 1f;
    [SerializeField] private float clickMultiplier = 1f;

    [Header("상태별 생산 배수")]
    [SerializeField] private float coolAutoMultiplier = 2f;
    [SerializeField] private float coolClickMultiplier = 3f;

    [SerializeField] private float unCoolAutoMultiplier = 0.5f;
    [SerializeField] private float unCoolClickMultiplier = 0.5f;

    [Header("로그 문장")]
    [SerializeField] private string coolText = "자동 배수 2배 적용";
    [SerializeField] private string unCoolText = "자동 배수 0.5배 적용";

    public float AutoMultiplier => autoMultiplier;
    public float ClickMultiplier => clickMultiplier;
    public float CoolAutoMultiplier => coolAutoMultiplier;
    public float CoolClickMultiplier => coolClickMultiplier;
    public float UnCoolAutoMultiplier => unCoolAutoMultiplier;
    public float UnCoolClickMultiplier => unCoolClickMultiplier;
    public string CoolText => coolText;
    public string UnCoolText => unCoolText;
}
