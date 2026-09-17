using UnityEngine;

[CreateAssetMenu(fileName = "BurningEventData", menuName = "Event/BurningEventData")]
public class BurningEventData : EventFactoryData
{
    [Header("버닝 타임 지속 시간")]
    [SerializeField] private float burningTime = 10f;

    [Header("버닝 타임 효과")]
    [SerializeField] private float clickMultiplier = 5f;
    [SerializeField] private float autoMultiplier = 2f;

    [Header("버닝 타임 종료")]
    [SerializeField] private float baseClickMultiplier = 1f;
    [SerializeField] private float baseAutoMultiplier = 1f;

    [Header("버닝 타임 종료 유예 시간")]
    [SerializeField] private float endTime = 2f;

    [Header("버닝 타임 UI 텍스트")]
    [SerializeField] private string burningStartText;
    [SerializeField] private string burningEndText;

    public float BurningTime => burningTime;
    public float ClickMultiplier => clickMultiplier;
    public float AutoMultiplier => autoMultiplier;
    public float BaseClickMultiplier => baseClickMultiplier;
    public float BaseAutoMultiplier => baseAutoMultiplier;
    public float EndTime => endTime;
    public string BurningStartText => burningStartText;
    public string BruningEndText => burningEndText;

    public override IEvent CreateEvent()
    {
        return new BurningEvent(this);
    }
}
