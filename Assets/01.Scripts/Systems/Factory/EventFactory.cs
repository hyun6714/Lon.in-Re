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
        createDic = new Dictionary<GameEventType, Func<IEvent>>();
                
        foreach (EventFactoryData eventData in data.EventDataList)
        {
            if (eventData == null)
                continue;

            if (createDic.ContainsKey(eventData.EventType))
            {
                Utils.Log($"중복된 이벤트 타입 : {eventData.EventType}");
                continue;
            }

            // 데이터 안의 리스트를 꺼내와서 타입을 Key값으로, 이벤트 실행 함수를 Value로 등록
            createDic.Add(eventData.EventType, eventData.CreateEvent);
        }
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
