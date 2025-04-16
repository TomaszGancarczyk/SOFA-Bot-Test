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
            DayOfWeek.Monday,
            DayOfWeek.Sunday
        ];
        private static readonly List<DayOfWeek> BaseCaptureDays =
        [
            DayOfWeek.Sunday
        ];
        internal static ComponentBuilder CreateQuestionButton()
        {
            DayOfWeek eventDayOfWeek = Timer.GetEventDateTime().DayOfWeek;
            ComponentBuilder component = new();
            if (TournamentDays.Contains(eventDayOfWeek))
            {
                component.WithButton("Tournament", "tournamentButton", emote: new Emoji("⚔️"));
            }
            if (GoldenDropDays.Contains(eventDayOfWeek))
            {
                component.WithButton("Golden Drop", "goldenDropButton", emote: new Emoji("📦"));
            }
            if (BaseCaptureDays.Contains(eventDayOfWeek))
            {
                component.WithButton("Base Capture", "baseCaptureButton", emote: new Emoji("👑"));
            }
            component.WithButton("Day Off", "dayOffButton", emote: new Emoji("🏖️"));
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
