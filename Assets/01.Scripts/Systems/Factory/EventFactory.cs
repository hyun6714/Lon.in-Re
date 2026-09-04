using System;
using System.Collections.Generic;

public class EventFactory
{
    private Dictionary<Season, Func<IEvent>> createDic;

    private SummerEventData summerData;

    public EventFactory(SummerEventData summerData)
    {
        this.summerData = summerData;

        InitFactory();
    }

    private void InitFactory()
    {
        createDic = new Dictionary<Season, Func<IEvent>>()
        {
            { Season.Spring, () => new SpringEvent() },
            { Season.Summer, () => new SummerEvent(summerData) },
            { Season.Fall, () => new FallEvent() },
            { Season.Winter, () => new WinterEvent() }
        };
    }

    public IEvent CreateEvent(Season season)
    {
        if (createDic.TryGetValue(season, out Func<IEvent> func))
        {
            return func?.Invoke();
        }

        Utils.Log("등록되지 않은 이벤트");
        return null;
    }
}
