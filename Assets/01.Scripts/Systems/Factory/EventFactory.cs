using System;
using System.Collections.Generic;

public class EventFactory
{
    private Dictionary<GameEventType, Func<IEvent>> createDic;

    private AirConditionalEventData airconData;

    public EventFactory(AirConditionalEventData airconData)
    {
        this.airconData = airconData;

        InitFactory();
    }

    private void InitFactory()
    {
        createDic = new Dictionary<GameEventType, Func<IEvent>>()
        {
            { GameEventType.Burning, () => new BurningEvent() },
            { GameEventType.AirConditional, () => new AirConditionalEvent(airconData) },
            { GameEventType.FallEvent, () => new FallEvent() },
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
