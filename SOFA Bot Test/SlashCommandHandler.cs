using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SOFA_Bot_Test.PlayerStats;

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
                    command.DeferAsync();
                    string playerName = command.Data.Options.First().Value.ToString();
                    logger.LogInformation("{Time} - User {user} asked for stats for {player}", DateTime.Now, command.User.GlobalName, playerName);
                    string player = await ApiHandler.GetPlayerStats(playerName);
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
