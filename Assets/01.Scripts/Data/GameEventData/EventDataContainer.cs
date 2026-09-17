using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EventDataContainer", menuName = "Event/EventDataContainer")]
public class EventDataContainer : ScriptableObject
{
    [SerializeField] private List<EventFactoryData> eventDataList;
    
    public List<EventFactoryData> EventDataList => eventDataList;
}
