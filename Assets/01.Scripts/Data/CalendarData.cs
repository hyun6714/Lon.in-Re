using UnityEngine;

[CreateAssetMenu(fileName = "CalendarBaseData", menuName = "Game/CalendarBaseData")]
public class CalendarData : ScriptableObject
{
    [Header("날짜 초기값")]
    [SerializeField] private int startYear = 1;
    [SerializeField] private int startMonth = 3;
    [SerializeField] private int startDay = 1;
    [SerializeField] private int startHour = 7;
    [SerializeField] private int startMinute = 0;
    [SerializeField] private Season startSeason = Season.Spring;

    [Header("시간 설정(n초당 m분)")]
    [SerializeField] private int realTime = 1;
    [SerializeField] private int minutePerSec = 15;

    [Header("이벤트 발생일")]
    [SerializeField] private int seasonEventStartDay = 15;

    public int StartYear => startYear;
    public int StartMonth => startMonth;
    public int StartDay => startDay;
    public int StartHour => startHour;
    public int StartMinute => startMinute;
    public Season StartSeason => startSeason;
    public int RealTime => realTime;
    public int MinutePerSec => minutePerSec;
    public int SeasonEventStartDay => seasonEventStartDay;
}