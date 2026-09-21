using System;
using System.Collections.Generic;
using UnityEngine;

public struct GameDate : IEquatable<GameDate>
{
    public int year;
    public int month;
    public int day;
    public int hour;
    public int minutes;
    public Season season;
    public int lastDay;

    public int defaultDaysInMonth;
    public int maxMonthPerYear;
    public int maxHourPerDay;
    public int maxMinutePerHour;

    public int monthPerSeason;

    public List<int> daysInMonthList;
    public List<SeasonInfo> seasonList;

    public GameDate(CalendarData data)
    {
        year = data.StartYear;
        month = data.StartMonth;
        day = data.StartDay;
        hour = data.StartHour;
        minutes = data.StartMinute;
        season = data.StartSeason;

        defaultDaysInMonth = data.DefaultDaysInMonth;
        maxMonthPerYear = data.MaxMonthPerYear;
        maxHourPerDay = data.MaxHourPerDay;
        maxMinutePerHour = data.MaxMinutePerHour;

        monthPerSeason = data.MonthPerSeason;

        daysInMonthList = data.DaysInMonthList;
        seasonList = data.SeasonList;

        lastDay = daysInMonthList[month - 1];
    }

    public void LoadDate(GameDateSaveData saveData)
    {
        year = saveData.year;
        month = saveData.month;
        day = saveData.day;
        hour = saveData.hour;
        minutes = saveData.minutes;

        UpdateSeason();
        lastDay = daysInMonthList[month - 1];
    }

    public void NextDay()
    {
        day++;

        if (day > lastDay)
        {
            day = 1;
            NextMonth();
        }
    }

    public void NextMonth()
    {
        month++;

        if (month > maxMonthPerYear)
        {
            month = 1;
            NextYear();
        }

        lastDay = daysInMonthList[month - 1];

        UpdateSeason();
    }

    public void NextYear()
    {
        year++;
    }

    public void UpdateSeason()
    {
        season = Season.Winter;

        // info에 있는 Season enum 값이 현재 달보다 작거나 같을 떄 계절 변경
        foreach (SeasonInfo info in seasonList)
        {
            if ((int)info.season <= month)
            {
                season = info.season;
            }
        }
    }

    public GameDate GetAfterDay(int addDay)
    {
        GameDate date = this;

        for (int i = 0; i < addDay; i++)
        {
            date.NextDay();
        }

        return date;
    }

    public readonly bool EqualMonthDayHour(GameDate other)
    {
        return month == other.month && day == other.day && hour == other.hour;
    }

    public readonly bool EqualMonthDay(GameDate other)
    {
        return month == other.month && day == other.day;
    }

    /// <summary>
    /// 현재 날짜 기준 남은 날짜 계산 함수
    /// </summary>
    /// <param name="targetDate"> 목표 날짜 </param>
    /// <param name="day"> 남은 일 수 </param>
    /// <param name="hour"> 남은 시간 </param>
    public readonly void GetRemainingTime(GameDate targetDate, out int day, out int hour)
    {
        int totalMinutes = GetRemainingMinutes(targetDate);
        int minutesPerDay = maxHourPerDay * maxMinutePerHour;

        day = totalMinutes / minutesPerDay;
        hour = (totalMinutes % minutesPerDay) / maxMinutePerHour;
    }

    /// <summary>
    /// 현재 날짜 기준 targetDate 날짜 확인
    /// </summary>
    /// <param name="targetDate"> 목표 날짜 </param>
    /// <returns></returns>
    public readonly int GetRemainingMinutes(GameDate targetDate)
    {
        if (targetDate <= this)
            return 0;

        GameDate currentDate = this;
        int totalMinutes = 0;

        int minutesPerHour = maxMinutePerHour;
        int minutesPerDay = maxHourPerDay * minutesPerHour;

        if (currentDate.year == targetDate.year && currentDate.month == targetDate.month
            && currentDate.day == targetDate.day)
        {
            return (targetDate.hour * minutesPerHour + targetDate.minutes) - (currentDate.hour * minutesPerHour + currentDate.minutes);
        }

        totalMinutes += minutesPerDay - (currentDate.hour * minutesPerHour + currentDate.minutes);

        currentDate.NextDay();

        currentDate.hour = 0;
        currentDate.minutes = 0;

        while (currentDate.year != targetDate.year || currentDate.month != targetDate.month
            || currentDate.day != targetDate.day)
        {
            totalMinutes += minutesPerDay;
            currentDate.NextDay();

            currentDate.hour = 0;
            currentDate.minutes = 0;
        }

        totalMinutes += targetDate.hour * minutesPerHour + targetDate.minutes;

        return totalMinutes;
    }

#if UNITY_EDITOR
    public void SetDate(GameDate date)
    {
        year = date.year;
        month = Mathf.Clamp(date.month, 1, maxMonthPerYear);

        lastDay = daysInMonthList[this.month - 1];

        day = Mathf.Clamp(date.day, 1, lastDay);
        hour = Mathf.Clamp(date.hour, 0, maxHourPerDay - 1);
        minutes = 0;

        SetSeason();
    }

    public void SetSeason()
    {
        season = month switch
        {
            >= 3 and <= 5 => Season.Spring,
            >= 6 and <= 8 => Season.Summer,
            >= 9 and <= 11 => Season.Fall,
            _ => Season.Winter
        };
    }
#endif
    #region 연산자 오버로딩
    public readonly bool Equals(GameDate other)
    {
        return year == other.year && month == other.month && day == other.day && hour == other.hour && minutes == other.minutes;
    }

    public readonly int CompareTo(GameDate other)
    {
        if (year > other.year)
            return 1;
        if (year < other.year)
            return -1;

        if (month > other.month)
            return 1;
        if (month < other.month)
            return -1;

        if (day > other.day)
            return 1;
        if (day < other.day)
            return -1;

        if (hour > other.hour)
            return 1;
        if (hour < other.hour)
            return -1;

        if (minutes > other.minutes)
            return 1;
        if (minutes < other.minutes)
            return -1;

        return 0;
    }
    
    public static bool operator ==(GameDate a, GameDate b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(GameDate a, GameDate b)
    {
        return !(a.Equals(b));
    }

    public static bool operator >=(GameDate a, GameDate b)
    {
        return a.CompareTo(b) >= 0;
    }

    public static bool operator <=(GameDate a, GameDate b)
    {
        return a.CompareTo(b) <= 0;
    }

    public static bool operator >(GameDate a, GameDate b)
    {
        return a.CompareTo(b) > 0;
    }

    public static bool operator <(GameDate a, GameDate b)
    {
        return a.CompareTo(b) < 0;
    }
    #endregion
}
