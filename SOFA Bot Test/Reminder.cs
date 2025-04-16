using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class Reminder
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");
        internal static async void Handle(TimeSpan reminderTimeSpan)
        {
            Task.Delay(reminderTimeSpan).Wait();
            string eventType = CreateMessage.GetEventType();
            List<SocketGuildUser> offMembers = [];
            if (eventType != "Day Off" && eventType != "Golden Drop")
            {
                foreach (var member in MemberHandler.GetSofaMembers())
                    if (member.Value == null) offMembers.Add(member.Key);
                foreach (var member in MemberHandler.GetUnassignedMembers())
                    offMembers.Add(member.Key);
            }
            foreach (var member in offMembers)
                SendReminder(member.Id);
        }
        private static void SendReminder(ulong userId)
        {
            //TODO continue here
        }
    }
}
