using UnityEngine;

[CreateAssetMenu(fileName = "New Exchange Data", menuName = "GameData/ExchangeData")]
public class ExchangeDataSO : ScriptableObject
{
    [Header("바꿀 재화")]
    [SerializeField] private CurrencyType sourceCurrencyType;     // 소모할 재화 타입 
    [SerializeField] private CurrencyType targetCurrencyType;     // 획득할 재화 타입 

    [Header("소모량/획득량")]
    [SerializeField] private long costAmount;                  
    [SerializeField] private long rewardAmount;                   

    // 외부에서 안전하게 읽을 수 있도록 프로퍼티 제공
    public CurrencyType SourceCurrencyType => sourceCurrencyType;
    public CurrencyType TargetCurrencyType => targetCurrencyType;
    public long CostAmount => costAmount;
    public long RewardAmount => rewardAmount;
}