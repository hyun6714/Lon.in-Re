using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEventData", menuName = ("Event/GameEventData"))]
public class GameEventData : ScriptableObject
{
    [Header("이벤트 목록")]
    [SerializeField] private List<GameEventInfo> eventList;

    public List<GameEventInfo> EventList => eventList;
}
