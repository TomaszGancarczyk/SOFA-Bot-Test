using Discord;
using Discord.WebSocket;

using System.Net;

namespace SOFA_Bot_Test.Attendance
{
    internal class Reminder
    {
        private static bool ReminderPermission = false;
        internal static Task SetReminderPermission(bool status)
        {
            Logger.LogInformation($"Setting reminders to {status}");
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
            Logger.LogInformation($"Sending reminder to {member.DisplayName}");
            string message = $"Don't forget to signup for {eventType} :3";
            IDMChannel channel = await member.CreateDMChannelAsync();
            try
            {
                await channel.SendMessageAsync(message);
            }
            catch (Discord.Net.HttpException ex) when (ex.HttpCode == HttpStatusCode.Forbidden)
            {
                Logger.LogWarning($"Cannot send reminder message to {member.DisplayName}");
            }
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
