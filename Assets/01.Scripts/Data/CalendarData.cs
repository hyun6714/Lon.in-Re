using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SeasonInfo
{
    public Season season;
    public string seasonName;
}

[CreateAssetMenu(fileName = "CalendarBaseData", menuName = "Game/CalendarBaseData")]
public class CalendarData : ScriptableObject
{
    [Header("기본 설정")]
    [SerializeField] private int defaultDaysInMonth = 30;

    [Header("날짜 초기값")]
    [SerializeField] private int startYear = 1;
    [SerializeField] private int startMonth = 3;
    [SerializeField] private int startDay = 1;
    [SerializeField] private int startHour = 7;
    [SerializeField] private int startMinute = 0;
    [SerializeField] private Season startSeason = Season.Spring;
    [SerializeField]
    private List<int> daysInMonthList = new List<int>()
    {
        31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31
    };

    [Header("시간 설정(n초당 m분)")]
    [SerializeField] private int realTime = 1;
    [SerializeField] private int minutePerSec = 15;

    [Header("시간 단위")]
    [SerializeField] private int maxMonthPerYear = 12;
    [SerializeField] private int maxHourPerDay = 24;
    [SerializeField] private int maxMinutePerHour = 60;

    [Header("계절")]
    [SerializeField] private int monthPerSeason = 3;
    [SerializeField] private List<SeasonInfo> seasonList;

    [Header("이벤트 시작 시간")]
    [SerializeField] private int baseEventHour = 7;

    public int DefaultDaysInMonth => defaultDaysInMonth;
    public int StartYear => startYear;
    public int StartMonth => startMonth;
    public int StartDay => startDay;
    public int StartHour => startHour;
    public int StartMinute => startMinute;
    public Season StartSeason => startSeason;
    public List<int> DaysInMonthList => daysInMonthList;
    public int RealTime => realTime;
    public int MinutePerSec => minutePerSec;
    public int MaxMonthPerYear => maxMonthPerYear;
    public int MaxHourPerDay => maxHourPerDay;
    public int MaxMinutePerHour => maxMinutePerHour;
    public int MonthPerSeason => monthPerSeason;
    public List<SeasonInfo> SeasonList => seasonList;
    public int BaseEventHour => baseEventHour;
}