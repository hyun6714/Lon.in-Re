using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GiftEventData", menuName = "Event/GiftEventData")]
public class GiftEventData : EventFactoryData
{
    [Header("등급별 수령 금액")]
    [SerializeField] private int solo = 500;
    [SerializeField] private int indie = 2000;
    [SerializeField] private int small = 10000;
    [SerializeField] private int midsized = 50000;
    [SerializeField] private int major = 200000;
    [SerializeField] private List<GiftRewardInfo> reward;
    
    public int Solo => solo;
    public int Indie => indie;
    public int Small => small;
    public int Midsized => midsized;
    public int Major => major;
    public List<GiftRewardInfo> Reward => reward;

    public override IEvent CreateEvent()
    {
        return new GiftEvent(this);
    }
}
