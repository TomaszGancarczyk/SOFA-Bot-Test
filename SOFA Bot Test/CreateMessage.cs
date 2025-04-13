using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Data;

namespace SOFA_Bot_Test
{
    internal class CreateMessage
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");
        private static readonly Emoji OffEmoji = "⚫";
        //TODO create messages
        internal async static Task<EmbedBuilder> CreateTournamentMessage()
        {
            logger.LogInformation("{Time} - Creating tournament message", DateTime.Now);
            var embed = CreateMessageTemplate("Tournament");
            SocketGuildUser[] sofaMembers = BotHandler.GetSofaMembers();
            for (int i = 1; i <= 7; i++)
            {
                string squadMembers = "";
                SocketRole role = BotHandler.GetRole($"Squad {i}");
                foreach (var member in sofaMembers)
                    if (member.Roles.Contains(role))
                        squadMembers += $"{OffEmoji} {member.DisplayName}\n";
                embed.AddField($"Squad {i}", squadMembers, true);
            }
            return embed;
        }
        internal async static Task<EmbedBuilder> CreateBaseCaptureMessage()
        {
            logger.LogInformation("{Time} - Creating base capture message", DateTime.Now);
            var embed = CreateMessageTemplate("Base Capture");
            SocketGuildUser[] sofaMembers = BotHandler.GetSofaMembers();
            for (int i = 1; i <= 7; i++)
            {
                string squadMembers = "";
                SocketRole role = BotHandler.GetRole($"Squad {i}");
                foreach (var member in sofaMembers)
                    if (member.Roles.Contains(role))
                        squadMembers += $"{OffEmoji} {member.DisplayName}\n";
                embed.AddField($"Squad {i}", squadMembers, true);
            }
            return embed;
        }
        internal async static Task<EmbedBuilder> CreateGoldenDropMessage()
        {
            logger.LogInformation("{Time} - Creating golden drop message", DateTime.Now);
            var embed = CreateMessageTemplate("Golden Drop");
            SocketGuildUser[] sofaMembers = BotHandler.GetSofaMembers();
            string sofaField = "";
            foreach (var member in sofaMembers)
            {
                sofaField += $"{OffEmoji} {member.DisplayName}\n";
            }
            SocketGuildUser[] rofaMembers = BotHandler.GetRofaMembers();
            string rofaField = "";
            foreach (var member in rofaMembers)
            {
                rofaField += $"{OffEmoji} {member.DisplayName}\n";
            }
            embed
                .AddField("SOFA", $"{sofaField}", true)
                .AddField("ROFA", $"{rofaField}", true);
            return embed;
        }
        private static EmbedBuilder CreateMessageTemplate(string eventType)
        {
            EmbedBuilder embed = new() { };
            DateTime eventDateTime = Timer.GetEventDateTime();
            long eventUnix = ((DateTimeOffset)eventDateTime).ToUnixTimeSeconds();
            embed
                .WithColor(Color.DarkGreen)
                .WithTitle($"{eventDateTime.DayOfWeek} {eventType}")
                .WithDescription($"<t:{eventUnix}:D><t:{eventUnix}:t> - <t:{eventUnix}:R>");
            return embed;
        }
    }
}
