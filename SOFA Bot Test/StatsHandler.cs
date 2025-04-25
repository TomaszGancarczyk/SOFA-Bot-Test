using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFA_Bot_Test
{
    internal class StatsHandler
    {
        internal async static Task<EmbedBuilder> GetStatsMessage(SocketSlashCommand command)
        {
            return null;
        }
        internal async static Task<EmbedBuilder> GetStatsErrorMessage()
        {
            EmbedBuilder embed = new();
            embed.WithColor(Color.Red);
            embed.WithTitle($"Couldn't find this player");
            return embed;
        }
    }
}
