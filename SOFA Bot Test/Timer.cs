﻿using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class Timer
    {
        private static DateTime EventDateTime;
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");
        internal static DateTime GetEventDateTime()
        {
            return EventDateTime;
        }
        internal static void SetEventDateTimeForNextDay()
        {
            DateOnly eventDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            TimeOnly eventTime = new(19, 00);
            DateTime eventDateTime = new(eventDate, eventTime);

            DateOnly lastDayOfMarch = new(DateTime.Now.Year, 3, 31);
            DateOnly cestStartDate = GetTimeChangeDateFromLastDayOfMonth(lastDayOfMarch);
            DateOnly lastDayOfOctober = new(DateTime.Now.Year, 10, 31);
            DateOnly cetStartDate = GetTimeChangeDateFromLastDayOfMonth(lastDayOfOctober);
            TimeOnly timeChangeTime = new(1, 00);
            if (eventDateTime > cestStartDate.ToDateTime(timeChangeTime) && eventDateTime < cetStartDate.ToDateTime(timeChangeTime))
            {
                eventTime.AddHours(1);
            }
            EventDateTime = eventDateTime;
        }
        private static DateOnly GetTimeChangeDateFromLastDayOfMonth(DateOnly date)
        {
            if (date.DayOfWeek != DayOfWeek.Sunday)
            {
                return date.AddDays(-(int)date.DayOfWeek);
            }
            return date;
        }

        internal static TimeSpan GetEventTimeSpan()
        {
            DateTime eventDateTime = Timer.GetEventDateTime();
            TimeSpan eventTimeSpan = eventDateTime - DateTime.Now;
            logger.LogInformation("{Time} - Remaining time: {eventTimeSpan}", DateTime.Now, eventTimeSpan);

            return eventTimeSpan;
        }
    }
}
