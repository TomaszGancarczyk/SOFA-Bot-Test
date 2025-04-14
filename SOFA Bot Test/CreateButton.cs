using Discord;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    public class CreateButton
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");
        private static readonly List<DayOfWeek> TournamentDays =
        [
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday
        ];
        private static readonly List<DayOfWeek> GoldenDropDays =
        [
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday
        ];
        private static readonly List<DayOfWeek> BaseCaptureDays =
        [
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday
        ];
        internal static ComponentBuilder CreateQuestionButton()
        {
            DayOfWeek eventDayOfWeek = Timer.GetEventDateTime().DayOfWeek;
            ComponentBuilder component = new ComponentBuilder();
            if (TournamentDays.Any(day => day == eventDayOfWeek))
            {
                component.WithButton("Tournament", "tournamentButton");
            }
            if (GoldenDropDays.Any(day => day == eventDayOfWeek))
            {
                component.WithButton("Golden Drop", "goldenDropButton");
            }
            if (BaseCaptureDays.Any(day => day == eventDayOfWeek))
            {
                component.WithButton("Base Capture", "baseCaptureButton");
            }
            component.WithButton("Day Off", "dayOffButton");
            return component;
        }
        internal static ComponentBuilder CreateAttendanceButton()
        {
            ComponentBuilder button = new ComponentBuilder()
                .WithButton("Present", "presentButton", ButtonStyle.Success)
                .WithButton("Absent", "absentButton", ButtonStyle.Danger);
            logger.LogInformation("{Time} - Created buttons", DateTime.Now);
            return button;
        }
    }
}
