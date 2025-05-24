using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test.Attendance
{
    internal class Reminder
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Attendance");
        private static bool ReminderPermission = false;
        internal static Task SetReminderPermission(bool status)
        {
            logger.LogInformation("{Time} - Setting reminders to {status}", DateTime.Now, status);
            ReminderPermission = status;
            return Task.CompletedTask;
        }
        internal static async void Handle()
        {
            if (ReminderPermission)
            {
                string eventType = SignupMessage.GetEventType();
                List<SocketGuildUser> offMembers = [];
                if (eventType == "Tournament" || eventType == "Base Capture")
                {
                    foreach (var member in MemberHandler.GetSofaMembers())
                        if (member.Value == null)
                            offMembers.Add(member.Key);
                    foreach (var member in MemberHandler.GetUnassignedMembers())
                        if (member.Value == null)
                            offMembers.Add(member.Key);
                }
                foreach (var member in offMembers)
                    await SendReminder(member, eventType);
            }
        }
        private static async Task SendReminder(SocketGuildUser member, string eventType)
        {
            logger.LogInformation("{Time} - Sending reminder to {member}", DateTime.Now, member.DisplayName);
            string message = $"Don't forget to signup for {eventType} :3";
            await member.SendMessageAsync(message);
        }
        internal static async Task<EmbedBuilder> CreateSignupCommandResponse(bool status)
        {
            EmbedBuilder embed = new();
            if (status)
            {
                embed.WithColor(Color.Green);
            }
            else if (!status)
            {
                embed.WithColor(Color.Red);
            }
            embed.WithTitle($"Set reminder status to {status}");
            return embed;
        }
    }
}
