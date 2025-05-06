using Discord;
using Discord.WebSocket;

namespace SOFA_Bot_Test.PlayerStats
{
    internal class StatsHandler
    {
        internal async static Task<EmbedBuilder> CreateStatsMessage(SocketSlashCommand command, PlayerStatsDeserialized player)
        {
            EmbedBuilder embed = new();
            embed.WithColor(Color.Green);
            return embed;
        }
        internal async static Task<EmbedBuilder> CreateStatsErrorMessage(SocketSlashCommand command)
        {
            EmbedBuilder embed = new();
            embed.WithColor(Color.Red);
            embed.WithTitle($"Couldn't find player {command.Data.Options.First().Value.ToString}");
            return embed;
        }
    }
}
