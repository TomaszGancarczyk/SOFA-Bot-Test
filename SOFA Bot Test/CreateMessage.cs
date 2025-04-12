using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class CreateMessage
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");
        private static readonly Emoji offEmoji = "⚫";
        //TODO create messages
        internal async static Task<EmbedBuilder> CreateTournamentMessage()
        {
            logger.LogInformation("{Time} - Creating tournament message", DateTime.Now);
            var embed = CreateMessageTemplate("Tournament");
            SocketGuildUser[] sofaMembers = BotHandler.GetSofaMembers();
            embed.AddField("Sqquad 1",
                "field 1 test", true)
                .AddField("Sqquad 2",
                "field 2 test", true)
                .AddField("Sqquad 3",
                "field 3 test", true)
                .WithFooter(footer => footer.Text = "footer test");
            return embed;
        }
        internal async static Task<EmbedBuilder> CreateBaseCaptureMessage()
        {
            logger.LogInformation("{Time} - Creating base capture message", DateTime.Now);
            var embed = CreateMessageTemplate("Base Capture");
            SocketGuildUser[] sofaMembers = BotHandler.GetSofaMembers();
            embed.AddField("Sqquad 1",
                "field 1 test", true)
                .AddField("Sqquad 2",
                "field 2 test", true)
                .AddField("Sqquad 3",
                "field 3 test", true)
                .WithFooter(footer => footer.Text = "footer test");
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
                sofaField += $"{offEmoji} {member.DisplayName}\n";
            }
            SocketGuildUser[] rofaMembers = BotHandler.GetRofaMembers();
            string rofaField = "";
            foreach (var member in rofaMembers)
            {
                rofaField += $"{offEmoji} {member.DisplayName}\n";
            }
            embed.AddField("SOFA", $"{sofaField}", true)
                .AddField("ROFA", $"{rofaField}", true)
                .WithFooter(footer => footer.Text = "footer test");
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
