using System.Collections.Generic;
using UnityEngine;

public class GiftEvent : IEvent
{
    private GiftEventData data;

    private Dictionary<RankManager.RankState, int> rankMoney = new Dictionary<RankManager.RankState, int>();

    public GiftEvent(GiftEventData data)
    {
        this.data = data;
    }

    public void StartEvent()
    {

    }

    private void InitDic()
    {
        
    }

    public void RecieveGift()
    {
        if (RankManager.instance == null)
            return;

        RankManager.RankState rank = RankManager.instance.currentRank;

        if (!rankMoney.TryGetValue(rank, out int amount))
            return;

        GameEventBridge.CurrencyAdded(CurrencyType.Normal, amount);
        EventManager.instance.EndCurrentEvent(GameEventType.Gift);
    }

    public void EndEvent()
    {

    }

    public void SaveEventData(EventSaveData eventSaveData)
    {

    }
}
