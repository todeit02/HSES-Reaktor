using System;

public class WeekbasedDateTimeInterval
{
    // January 1st of year 1 was a Monday.
    private const Int32 baseYear = 1;
    private const Int32 baseMonth = 1;
    private const Int32 baseSeconds = 0;

    private DateTime start;
    private DateTime end;

    public WeekbasedDateTimeInterval(DayOfWeek startDay, Int32 startHours, Int32 startMinutes, DayOfWeek endDay, Int32 endHours, Int32 endMinutes)
    {
        Int32 settingDay;

        settingDay = ToIntBaseMonday(startDay) + 1;
        start = new DateTime(baseYear, baseMonth, settingDay, startHours, startMinutes, baseSeconds);

        settingDay = ToIntBaseMonday(endDay) + 1;
        start = new DateTime(baseYear, baseMonth, settingDay, endHours, endMinutes, baseSeconds);
    }

    public bool Contains(DateTime checkingDateTime)
    {
        bool isBeforeStart = checkingDateTime.CompareTo(start) < 0;
        bool isAfterEnd = checkingDateTime.CompareTo(end) > 0;

        return (!isBeforeStart && !isAfterEnd);
    }

    private static Int32 ToIntBaseMonday(DayOfWeek convertingDay)
    {
        const Int32 baseSunday = 6;

        if (convertingDay != DayOfWeek.Sunday)
        {
            return (Int32)convertingDay - 1;
        }
        else
        {
            return baseSunday;
        }
    }
}
