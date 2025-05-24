using Discord;
using Discord.WebSocket;
using System.Reactive.Joins;

namespace SOFA_Bot_Test.PlayerStats
{
    internal class StatsHandler
    {
        internal async static Task<EmbedBuilder> CreateStatsMessage(Stats player)
        {
            EmbedBuilder embed = new();
            IEmote factionEmote = new Emoji("⚫");
            switch (player.Faction)
            {
                case "Rise":
                    embed.WithColor(Color.Green);
                    factionEmote = new Emoji("🟢");
                    break;
                case "Frontier":
                    embed.WithColor(Color.Red);
                    factionEmote = new Emoji("🔴");
                    break;
                case "Covenant":
                    embed.WithColor(Color.Purple);
                    factionEmote = new Emoji("🟣");
                    break;
                case "Mercenaries":
                    embed.WithColor(Color.Blue);
                    factionEmote = new Emoji("🔵");
                    break;
            }
            string clanString = "";
            string clansJoinedString = "";
            EmbedFieldBuilder GameTimeField = new EmbedFieldBuilder()
            {
                Name = $"Game Time",
                Value = $"- {player.PlaytimeHours} hours since {player.JoinedGame}\n" +
                $"- Last login: {player.LastLogin}",
                IsInline = false,
            };
            EmbedFieldBuilder PveField = new EmbedFieldBuilder()
            {
                Name = $"PVE",
                Value = $"- NPC Kills: {player.NpcKills}\n" +
                $"- MutantKills: {player.MutantKills}",
                IsInline = false,
            };
            EmbedFieldBuilder PvpField = new EmbedFieldBuilder()
            {
                Name = $"PVP",
                Value = $"- Kills: {player.Kills}\n" +
                $"- Deaths: {player.Deaths}\n" +
                $"- Assists: {player.Assists}\n" +
                $"- Suicides: {player.Suicides}\n" +
                $"- Total K/D: {player.TotalKD}\n" +
                $"- Session K/D: {player.SessionKD}",
                IsInline = false,
            };
            EmbedFieldBuilder OtherField = new EmbedFieldBuilder()
            {
                Name = $"Other",
                Value = $"- Highest Money: {player.HighestMoney}\n" +
                $"- Artifacts Found: {player.ArtifactsFound}\n" +
                $"- Bolts Thrown: {player.BoltsThrown}\n" +
                $"- Deliveries Made: {player.DeliveriesMade}\n" +
                $"- Caches Found: {player.CachesFound}\n" +
                $"- Signals Found: {player.SignalsFound}",
                IsInline = false,
            };

            if (player.Clan != null)
                clanString = $"**{player.ClanRank} of {player.ClanTag} {player.Clan}**\n";
            if (player.TimesJoinedClan != null)
                clansJoinedString = $"and member of {player.TimesJoinedClan - 1} clans before that\n";
            PveField.Name = "PVE";
            PvpField.Name = "PVP";
            OtherField.Name = "Other";

            embed
                .WithTitle($"{factionEmote} {player.Faction} **member** {player.Username}")
                .WithDescription($"{clanString}" + $"{clansJoinedString}")
                .WithFields(GameTimeField, PveField, PvpField, OtherField);

            return embed;
        }
        internal async static Task<EmbedBuilder> CreateStatsErrorMessage(SocketSlashCommand command)
        {
            EmbedBuilder embed = new();
            embed.WithColor(Color.Red)
                .WithTitle($"Couldn't find player {command.Data.Options.First().Value.ToString}");
            return embed;
        }
    }
}
