using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TreasureGoblinEventData", menuName = "Event/TreasureGoblinEventData")]
public class TreasureGoblinEventData : EventFactoryData
{
    [Header("이벤트 텍스트")]
    [SerializeField] private string eventStartText = "보물 고블린 등장!";
    [SerializeField] private string eventSuccessText = "보물 고블린을 잡았다!";
    [SerializeField] private string eventFailText = "보물 고블린이 도망갔다...";

    [Header("이벤트")]
    [SerializeField] private float eventTimer = 5f;
    [SerializeField] private int requireTouchCount = 15;

    [Header("이벤트 보상")]
    [SerializeField] private List<TreasureGoblinRewardInfo> reward;

    public string EventStartText => eventStartText;
    public string EventSuccessText => eventSuccessText;
    public string EventFailText => eventFailText;

    public float EventTimer => eventTimer;
    public int RequireTouchCount => requireTouchCount;

    public List<TreasureGoblinRewardInfo> Reward => reward;

    public override IEvent CreateEvent()
    {
        return new TreasureGoblinEvent(this);
    }
}
