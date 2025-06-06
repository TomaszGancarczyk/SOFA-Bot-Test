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
                    var sofaMembers = await MemberHandler.GetSofaMembers();
                    foreach (var member in sofaMembers)
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
            ulong? guildId = BotHandler.GetGuild().Id;
            ulong? signupsChannelId = BotHandler.GetSignupsChannelId();
            ulong? currentmessageId = BotHandler.GetCurrentMessageId();
            string reminderMessageLink = $"https://discord.com/channels/{guildId}/{signupsChannelId}/{currentmessageId}";
            if (guildId == null || signupsChannelId == null || currentmessageId == null)
            {
                Logger.LogError($"Cannot get data for reminder message");
            }
            else
            {
                string message = $"Don't forget to signup for {eventType} :3\n{reminderMessageLink}\nhttps://cdn.discordapp.com/attachments/1212775170087981066/1370073772257841255/9tau56.gif?ex=683d2762&is=683bd5e2&hm=c96633b107a2305067024a0ee72b43abd714f47a7bed503fcc30b19641f2cc05&";
                IDMChannel channel = await member.CreateDMChannelAsync();
                try
                {
                    await channel.SendMessageAsync(message);
                }
                catch (Discord.Net.HttpException ex) when (ex.HttpCode == HttpStatusCode.Forbidden)
                {
                    Logger.LogError($"Cannot send reminder message to {member.DisplayName}");
                }
                await channel.CloseAsync();
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
