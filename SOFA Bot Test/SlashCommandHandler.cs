using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace SOFA_Bot_Test
{
    internal class SlashCommandHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");
        public static async Task Handler(SocketSlashCommand command)
        {
            EmbedBuilder statsMessage;
            switch (command.Data.Name)
            {
                case "stats":
                    logger.LogInformation("{Time} - Got tournament response to event question", DateTime.Now);
                    statsMessage = await StatsHandler.GetStatsMessage(command);
                    if (statsMessage != null)
                    {
                        await command.RespondAsync(embed: statsMessage.Build(), ephemeral: false);
                    }
                    else
                    {
                        var errormessage = await StatsHandler.GetStatsErrorMessage();
                        await command.RespondAsync(embed: errormessage.Build(), ephemeral: true);
                    }
                    break;
            }
        }
    }
}
