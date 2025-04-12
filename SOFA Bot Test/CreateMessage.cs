using Discord;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata.Ecma335;

namespace SOFA_Bot_Test
{
    internal class CreateMessage
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");

        //TODO create messages
        internal async static Task<EmbedBuilder> CreateTournamentMessage()
        {
            logger.LogInformation("{Time} - Creating tournament message", DateTime.Now);
            var embed = CreateMessageTemplate("Tournament");
            embed.AddField("title 1 test",
                "field 1 test")
                .AddField("title 2 test",
                "field 2 test")
                .AddField("title 3 test",
                "field 3 test")
                .WithFooter(footer => footer.Text = "footer test");
            return embed;
        }
        internal async static Task<EmbedBuilder> CreateBaseCaptureMessage()
        {
            logger.LogInformation("{Time} - Creating base capture message", DateTime.Now);
            var embed = CreateMessageTemplate("Base Capture");
            embed.AddField("title 1 test",
                "field 1 test")
                .AddField("title 2 test",
                "field 2 test")
                .AddField("title 3 test",
                "field 3 test")
                .WithFooter(footer => footer.Text = "footer test");
            return embed;
        }
        internal async static Task<EmbedBuilder> CreateGoldenDropMessage()
        {
            logger.LogInformation("{Time} - Creating golden drop message", DateTime.Now);
            var embed = CreateMessageTemplate("Golden Drop");
            embed.AddField("title 1 test",
                "field 1 test")
                .AddField("title 2 test",
                "field 2 test")
                .AddField("title 3 test",
                "field 3 test")
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
