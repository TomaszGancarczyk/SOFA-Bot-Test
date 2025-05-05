using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test.Attendance
{
    internal class Reminder
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Attendance");
        private static bool ReminderPermission = false;
        internal static Task SetReminderPermission(bool permission)
        {
            ReminderPermission = permission;
            return Task.CompletedTask;
        }
        internal static async void Handle(TimeSpan reminderTimeSpan)
        {
            if (ReminderPermission)
            {
                Task.Delay(reminderTimeSpan).Wait();
                string eventType = CreateMessage.GetEventType();
                List<SocketGuildUser> offMembers = [];
                if (eventType != "Day Off" && eventType != "Golden Drop" && eventType != "Brawl")
                {
                    foreach (var member in MemberHandler.GetSofaMembers())
                        if (member.Value == null)
                            offMembers.Add(member.Key);
                    foreach (var member in MemberHandler.GetUnassignedMembers())
                        if (member.Value == null)
                            offMembers.Add(member.Key);
                }
                foreach (var member in offMembers)
                    await SendReminder(member);
            }
        }
        private static async Task SendReminder(SocketGuildUser member)
        {
            logger.LogInformation("{Time} - Sending reminder to {member}", DateTime.Now, member.DisplayName);
            string message = "Don't forget to signup :3";
            await member.SendMessageAsync(message);
        }
    }
}
