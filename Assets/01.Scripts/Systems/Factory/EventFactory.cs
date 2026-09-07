using System;
using System.Collections.Generic;

public class EventFactory
{
    private Dictionary<EventType, Func<IEvent>> createDic;

    private SummerEventData summerData;

    public EventFactory(SummerEventData summerData)
    {
        this.summerData = summerData;

        InitFactory();
    }

    private void InitFactory()
    {
        createDic = new Dictionary<EventType, Func<IEvent>>()
        {
            { EventType.SpringEvent, () => new SpringEvent() },
            { EventType.SummerEvent, () => new SummerEvent(summerData) },
            { EventType.FallEvent, () => new FallEvent() },
            { EventType.WinterEvent, () => new WinterEvent() }
        };
    }

    public IEvent CreateEvent(EventType type)
    {
        if (createDic.TryGetValue(type, out Func<IEvent> func))
        {
            return func?.Invoke();
        }

        Utils.Log("등록되지 않은 이벤트");
        return null;
    }
}
