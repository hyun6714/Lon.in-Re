using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameEventInfo
{
    [Header("이벤트 고유 ID")]
    [SerializeField] private GameEventType eventType;

    [Header("발생 조건(날짜)")]
    [SerializeField] private List<GameEventDate> gameEventDateList;

    [Header("팝업 이름")]
    [SerializeField] private UIName popupName;

    public GameEventType EventType => eventType;
    public List<GameEventDate> GameEventDateList => gameEventDateList;
    public UIName PopupName => popupName;
}

[Serializable]
public struct GameEventDate
{
    [SerializeField] private int month;
    [SerializeField] private int day;
    [SerializeField] private int hour;

    public int Month => month;
    public int Day => day;
    public int Hour => hour;
}