using Discord;
using Discord.WebSocket;

namespace SOFA_Bot_Test
{
    internal class StatsHandler
    {
        internal async static Task<EmbedBuilder> CreateStatsMessage(SocketSlashCommand command, string player)
        {
            EmbedBuilder embed = new();
            embed.WithColor(Color.Green);
            return embed;
        }
        internal async static Task<EmbedBuilder> CreateStatsErrorMessage(SocketSlashCommand command)
        {
            EmbedBuilder embed = new();
            embed.WithColor(Color.Red);
            embed.WithTitle($"Couldn't find this player");
            return embed;
        }
    }
}
