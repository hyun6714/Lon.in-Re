using System;
using System.Collections.Generic;

public class EventFactory
{
    private Dictionary<GameEventType, Func<IEvent>> createDic;

    private SummerEventData summerData;

    public EventFactory(SummerEventData summerData)
    {
        this.summerData = summerData;

        InitFactory();
    }

    private void InitFactory()
    {
        createDic = new Dictionary<GameEventType, Func<IEvent>>()
        {
            { GameEventType.SpringEvent, () => new SpringEvent() },
            { GameEventType.SummerEvent, () => new SummerEvent(summerData) },
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
