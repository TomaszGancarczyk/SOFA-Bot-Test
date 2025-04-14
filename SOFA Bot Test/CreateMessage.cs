using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class CreateMessage
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");
        private static readonly Emoji OffEmoji = "⚫";
        internal async static Task<EmbedBuilder> CreateAttendanceMessage(string eventType)
        {
            logger.LogInformation("{Time} - Creating {eventType} message", DateTime.Now, eventType);
            EmbedBuilder embed = new() { };
            DateTime eventDateTime = Timer.GetEventDateTime();
            long eventUnix = ((DateTimeOffset)eventDateTime).ToUnixTimeSeconds();
            switch (eventType)
            {
                case "Tournament":
                    embed.WithColor(Color.Green);
                    break;
                case "Golden Drop":
                    embed.WithColor(Color.Gold);
                    break;
                case "Base Capture":
                    embed.WithColor(Color.DarkGreen);
                    break;
            }
            embed
            .WithTitle($"{eventDateTime.DayOfWeek} {eventType}")
            .WithDescription($"<t:{eventUnix}:D><t:{eventUnix}:t> - <t:{eventUnix}:R>");
            BotHandler.SetSofaMembers();
            Dictionary<SocketGuildUser, bool?> sofaMembers = BotHandler.GetSofaMembers();
            BotHandler.SetRofaMembers();
            Dictionary<SocketGuildUser, bool?> rofaMembers = BotHandler.GetRofaMembers();
            if (eventType == "Golden Drop")
            {
                string sofaField = "";
                foreach (var member in sofaMembers.Keys)
                {
                    sofaField += $"{OffEmoji} {member.DisplayName}\n";
                }
                string rofaField = "";
                foreach (var member in rofaMembers.Keys)
                {
                    rofaField += $"{OffEmoji} {member.DisplayName}\n";
                }
                embed
                    .AddField("SOFA", $"{sofaField}", true)
                    .AddField("ROFA", $"{rofaField}", true);
            }
            else
            {
                for (int i = 1; i <= 7; i++)
                {
                    string squadMembers = "";
                    SocketRole role = BotHandler.GetRole($"Squad {i}");
                    foreach (var member in sofaMembers.Keys)
                        if (member.Roles.Contains(role))
                            squadMembers += $"{OffEmoji} {member.DisplayName}\n";
                    embed.AddField($"Squad {i}", squadMembers, true);
                }
            }
            return embed;
        }
    }
}
