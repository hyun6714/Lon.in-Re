using UnityEngine;

[CreateAssetMenu(fileName = "AutoProductionData", menuName = "Game/AutoProductionData")]
public class AutoProductionData : ScriptableObject
{
    [Header("생산량 설정")]
    [SerializeField] private long baseMoneyPerSec = 0;
    [SerializeField] private float baseAutoSec = 1f;

    [Header("생산량 텍스트")]
    [SerializeField] private string moneyPerSecText = "{0}G/초";
    
    public long BaseMoneyPerSec => baseMoneyPerSec;
    public float BaseAutoSec => baseAutoSec;
    public string MoneyPerSecText => moneyPerSecText;
}
