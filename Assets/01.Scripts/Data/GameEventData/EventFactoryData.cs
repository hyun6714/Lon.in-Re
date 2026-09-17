using UnityEngine;

public abstract class EventFactoryData : ScriptableObject
{
    [SerializeField] private GameEventType eventType;

    public GameEventType EventType => eventType;

    public abstract IEvent CreateEvent();
}
