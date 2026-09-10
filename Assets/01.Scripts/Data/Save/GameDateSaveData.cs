using System;

[Serializable]
public class GameDateSaveData
{
    public int year;
    public int month;
    public int day;
    public int hour;
    public int minutes;

    public GameDateSaveData(GameDate date)
    {
        year = date.year;
        month = date.month;
        day = date.day;
        hour = date.hour;
        minutes = date.minutes;
    }
}
