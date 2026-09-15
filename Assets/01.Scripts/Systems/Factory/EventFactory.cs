using System;
using System.Collections.Generic;

public class EventFactory
{
    private Dictionary<GameEventType, Func<IEvent>> createDic;

    private EventDataContainer data;

    public EventFactory(EventDataContainer container)
    {
        data = container;

        InitFactory();
    }

    private void InitFactory()
    {
        createDic = new Dictionary<GameEventType, Func<IEvent>>()
        {
            { GameEventType.Burning, () => new BurningEvent(data.burningData) },
            { GameEventType.AirConditional, () => new AirConditionalEvent(data.airconData) },
            { GameEventType.Gift, () => new GiftEvent(data.giftData) },
            { GameEventType.WinterEvent, () => new WinterEvent() }
        };
    }

    public IEvent CreateEvent(GameEventType type)
    {
        if (createDic.TryGetValue(type, out Func<IEvent> func))
        {
            return func?.Invoke();
        }

        Utils.Log("등록되지 않은 이벤트");
        return null;
    }
}
