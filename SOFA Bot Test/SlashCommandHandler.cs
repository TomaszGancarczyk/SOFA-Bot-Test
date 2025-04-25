using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class SlashCommandHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");
        public static async Task Handler(SocketSlashCommand command)
        {
            EmbedBuilder embed;
            bool isEphemeral;
            switch (command.Data.Name)
            {
                case "stats":
                    logger.LogInformation("{Time} - User {user} asked for stats for {player}", DateTime.Now, command.User.GlobalName, command.Data.Options.First().Value);
                    string player = null;
                    if (player != null)
                    {
                        embed = await StatsHandler.CreateStatsMessage(command, player);
                        isEphemeral = false;
                    }
                    else
                    {
                        embed = await StatsHandler.CreateStatsErrorMessage(command);
                        isEphemeral = true;
                    }
                    await command.FollowupAsync(embed: embed.Build(), ephemeral: isEphemeral);
                    break;
            }
        }
    }
}
