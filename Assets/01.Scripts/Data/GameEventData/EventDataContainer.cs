using UnityEngine;

[CreateAssetMenu(fileName = "EventDataContainer", menuName = "Event/EventDataContainer")]
public class EventDataContainer : ScriptableObject
{
    public BurningEventData burningData;
    public AirConditionalEventData airconData;
}
