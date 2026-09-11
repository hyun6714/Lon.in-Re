using System;
using System.Collections.Generic;

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

    public bool EqualMonthDay(GameDate other)
    {
        return month == other.month && day == other.day;
    }

#if UNITY_EDITOR
    public void SetDate(int year, int month, int day, int hour)
    {
        this.year = year;
        this.month = Math.Clamp(month, 1, maxMonthPerYear);

        lastDay = daysInMonthList[this.month - 1]; ;

        this.day = Math.Clamp(day, 1, lastDay);
        this.hour = Math.Clamp(hour, 0, maxHourPerDay - 1);
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
    public bool Equals(GameDate other)
    {
        return year == other.year && month == other.month && day == other.day && hour == other.hour && minutes == other.minutes;
    }

    public int CompareTo(GameDate other)
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
