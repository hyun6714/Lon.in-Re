using System;
using UnityEngine;

public struct EventDate : IEquatable<EventDate>
{
    public int month;
    public int day;
    
    public EventDate(int month, int day)
    {
        this.month = month;
        this.day = day;
    }

    public bool Equals(EventDate other)
    {
        return month == other.month && day == other.day;
    }

    public static bool operator ==(EventDate a, EventDate b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(EventDate a, EventDate b)
    {
        return !(a.Equals(b));
    }
}
