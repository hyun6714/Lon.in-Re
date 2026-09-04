using UnityEngine;

[CreateAssetMenu(fileName = "PartData", menuName = "Game/Part Data")]
public class PartData : ScriptableObject
{
    [SerializeField] private string partId;
    [SerializeField] private string partName;
    [SerializeField] private Sprite icon;

    [SerializeField] private RankManager.RankState unlockGrade; // 해금 조건을 넣으려면 사용

    [SerializeField] private int baseCost = 10;
    [SerializeField] private float costMultiplier = 1.3f;
    [SerializeField] private int powerPerLevel = 1;

    public string PartId => partId;
    public string PartName => partName;
    public Sprite Icon => icon;
    public RankManager.RankState UnlockGrade => unlockGrade;
    public int BaseCost => baseCost;
    public float CostMultiplier => costMultiplier;
    public int PowerPerLevel => powerPerLevel;
}
