using UnityEngine;

[CreateAssetMenu(fileName = "BurningEventData", menuName = "Event/BurningEventData")]
public class BurningEventData : ScriptableObject
{
    [Header("버닝 타임 지속 시간")]
    [SerializeField] private float burningTime = 10f;

    [Header("버닝 타임 효과")]
    [SerializeField] private float clickMultiplier = 5f;
    [SerializeField] private float autoMultiplier = 2f;

    [Header("버닝 타임 종료")]
    [SerializeField] private float baseClickMultiplier = 1f;
    [SerializeField] private float baseAutoMultiplier = 1f;

    public float BurningTime => burningTime;
    public float ClickMultiplier => clickMultiplier;
    public float AutoMultiplier => autoMultiplier;
    public float BaseClickMultiplier => baseClickMultiplier;
    public float BaseAutoMultiplier => baseAutoMultiplier;
}
