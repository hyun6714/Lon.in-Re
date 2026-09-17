using System.Collections.Generic;
using UnityEngine;

public class GiftEvent : IEvent
{
    private GiftEventData data;

    private Dictionary<RankManager.RankState, int> rankGold = new Dictionary<RankManager.RankState, int>();
    private Dictionary<RankManager.RankState, int> rankFame = new Dictionary<RankManager.RankState, int>();

    public GiftEvent(GiftEventData data)
    {
        this.data = data;
    }

    public void StartEvent()
    {
        InitDic();
    }

    private void InitDic()
    {
        foreach (GiftRewardInfo info in data.Reward)
        {
            if (!rankGold.TryAdd(info.rank, info.rewardGold))
            {
                Utils.Log($"이미 등록된 Gold 랭크 보상 : {info.rank}");
            }

            if (!rankFame.TryAdd(info.rank, info.rewardFame))
            {
                Utils.Log($"이미 등록된 Fame 랭크 보상 : {info.rank}");
            }
        }
    }

    public void ReceiveGift()
    {
        if (RankManager.instance == null)
            return;

        RankManager.RankState rank = RankManager.instance.currentRank;

        if (!rankGold.TryGetValue(rank, out int amountGold) ||
            !rankFame.TryGetValue(rank, out int amountFame))
            return;

        GameEventBridge.CurrencyAdded(CurrencyType.Normal, amountGold);
        GameEventBridge.CurrencyAdded(CurrencyType.Reputation, amountFame);

        EventManager.instance.EndCurrentEvent(GameEventType.Gift);
        UIManager.Instance.ClosePopup(UIName.Popup_Event_Gift);
    }

    public void EndEvent()
    {
        rankGold.Clear();
    }

    public void SaveEventData(EventSaveData eventSaveData)
    {

    }
}
