using System.Collections.Generic;
using UnityEngine;

public class GiftEvent : IEvent
{
    private GiftEventData data;

    private Dictionary<RankManager.RankState, int> rankMoney;

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
        rankMoney = new Dictionary<RankManager.RankState, int>()
        {
            { RankManager.RankState.Solo, data.Solo },
            { RankManager.RankState.Indie, data.Indie },
            { RankManager.RankState.Small, data.Small },
            { RankManager.RankState.Midsized, data.Midsized },
            { RankManager.RankState.MajorPublisher, data.Major }
        };
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
        UIManager.Instance.ClosePopup(UIName.Popup_Gift);
    }

    public void EndEvent()
    {
        rankMoney.Clear();
    }

    public void SaveEventData(EventSaveData eventSaveData)
    {

    }
}
