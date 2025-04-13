using Discord;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    public class CreateButton
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");
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
