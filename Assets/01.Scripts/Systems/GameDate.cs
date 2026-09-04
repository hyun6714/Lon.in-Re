using System;

public struct GameDate : IEquatable<GameDate>
{
    public int year;
    public int month;
    public int day;
    public int hour;
    public int minutes;
    public Season season;
    private int lastDay;

    public GameDate(CalendarData data)
    {
        year = data.StartYear;
        month = data.StartMonth;
        day = data.StartDay;
        hour = data.StartHour;
        minutes = data.StartMinute;
        season = data.StartSeason;
        lastDay = DateTime.DaysInMonth(year, month);
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

        if (month > 12)
        {
            month = 1;
            NextYear();
        }

        lastDay = DateTime.DaysInMonth(year, month);

        CheckNextSeason();
    }

    public void NextYear()
    {
        year++;
    }

    public void CheckNextSeason()
    {
        if (month % 3 == 0)
        {
            season = (Season)month;
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

    #region 연산자 오버로딩
    public static bool operator ==(GameDate a, GameDate b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(GameDate a, GameDate b)
    {
        return !a.Equals(b);
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
