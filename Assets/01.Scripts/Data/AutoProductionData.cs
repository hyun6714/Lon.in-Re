using UnityEngine;

[CreateAssetMenu(fileName = "AutoProductionData", menuName = "Game/AutoProductionData")]
public class AutoProductionData : ScriptableObject
{
    [Header("생산량 설정")]
    [SerializeField] private float baseMoneyPerSec = 0f;
    [SerializeField] private float baseAutoSec = 1f;
    
    public float BaseMoneyPerSec => baseMoneyPerSec;
    public float BaseAutoSec => baseAutoSec;
}
