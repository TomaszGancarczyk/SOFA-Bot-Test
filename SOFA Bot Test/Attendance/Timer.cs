namespace FOFA_Bot.Attendance
{
    internal class Timer
    {
        private static DateTime EventDateTime;
        internal static DateTime GetEventDateTime()
        {
            return EventDateTime;
        }
        internal static async Task SetEventDateTime(bool isToday)
        {
            DateOnly eventDate = DateOnly.FromDateTime(DateTime.Now);
            if (!isToday)
                eventDate = eventDate.AddDays(1);
            TimeOnly eventTime = new(19, 00);
            DateTime eventDateTime = new(eventDate, eventTime);

            DateOnly lastDayOfMarch = new(DateTime.Now.Year, 3, 31);
            DateOnly cestStartDate = GetTimeChangeDateFromLastDayOfMonth(lastDayOfMarch);
            DateOnly lastDayOfOctober = new(DateTime.Now.Year, 10, 31);
            DateOnly cetStartDate = GetTimeChangeDateFromLastDayOfMonth(lastDayOfOctober);
            TimeOnly timeChangeTime = new(1, 00);
            if (eventDateTime > cestStartDate.ToDateTime(timeChangeTime) && eventDateTime < cetStartDate.ToDateTime(timeChangeTime))
                eventDateTime = eventDateTime.AddHours(1);
            if (QuestionHandler.GetQuestionResponse() == "Base Capture")
            {
                Logger.LogInformation("Removing 1 hour for base capture");
                eventDateTime = eventDateTime.AddHours(-1);
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
    }
}
