using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GiftEventData", menuName = "Event/GiftEventData")]
public class GiftEventData : EventFactoryData
{
    [Header("등급별 수령 금액")]
    [SerializeField] private List<GiftRewardInfo> reward;
    
    public List<GiftRewardInfo> Reward => reward;

    public override IEvent CreateEvent()
    {
        return new GiftEvent(this);
    }
}
