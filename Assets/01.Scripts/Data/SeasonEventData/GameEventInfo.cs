using System;
using UnityEngine;

[Serializable]
public class GameEventInfo
{
    [Header("이벤트 고유 ID")]
    [SerializeField] private EventType eventType;

    [Header("발생 조건(날짜)")]
    [SerializeField] private int targetMonth;
    [SerializeField] private int targetDay;

    [Header("팝업 이름")]
    [SerializeField] private UIName popupName;

    public EventType EventType => eventType;
    public int TargetMonth => targetMonth;
    public int TargetDay => targetDay;
    public UIName PopupName => popupName;
}
