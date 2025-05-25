using System.Numerics;
using System.Reflection.Metadata.Ecma335;

namespace SOFA_Bot_Test.PlayerStats
{
    internal class Stats
    {
        internal string Uuid { get; set; }
        //image
        internal string Username { get; set; }
        internal string Faction { get; set; }
        internal string Clan { get; set; }
        internal string ClanTag { get; set; }
        internal string ClanRank { get; set; }
        internal int TimesJoinedClan { get; set; }
        internal int PlaytimeHours { get; set; }
        internal DateOnly JoinedGame { get; set; }
        internal DateOnly LastLogin { get; set; }
        internal int Kills { get; set; }
        internal int Deaths { get; set; }
        internal int Assists { get; set; }
        internal double TotalKD { get; set; }
        internal double SessionKD { get; set; }
        internal int Suicides { get; set; }
        internal int ArtifactsFound { get; set; }
        internal int HighestMoney { get; set; }
        //internal int MoneyMade { get; set; }
        internal int BoltsThrown { get; set; }
        internal int MutantKills { get; set; }
        internal int NpcKills { get; set; }
        internal int DeliveriesMade { get; set; }
        internal int CachesFound { get; set; }
        internal int SignalsFound { get; set; }

        internal static Stats CreateTestPlayer()
        {
            var player = new Stats
            {
                Uuid = "id",
                Username = "username",
                Faction = "Rise",
                Clan = "SOFA",
                ClanTag = "[TAG]",
                ClanRank = "Colonel",
                TimesJoinedClan = 5,
                PlaytimeHours = 500,
                JoinedGame = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                LastLogin = DateOnly.FromDateTime(DateTime.Now),
                Kills = 69,
                Deaths = 420,
                Assists = 1
            };
            player.TotalKD = player.Kills / player.Deaths;
            player.SessionKD = 10 / 500;
            player.Suicides = 10000;
            player.ArtifactsFound = 0;
            player.HighestMoney = 69420;
            player.BoltsThrown = 10;
            player.MutantKills = 10000000;
            player.NpcKills = 100;
            player.DeliveriesMade = 0;
            player.CachesFound = 2179846;
            player.SignalsFound = 54321;
            return player;
        }
    }




    //  PlayerStats
    //    public string Uuid { get; set; }
    //    public string Username { get; set; }
    //    public string Status { get; set; }
    //    public string Alliance { get; set; }
    //    public DateTime LastLogin { get; set; } //maybe string
    //    public List<string> DisplayedAchievements { get; set; }
    //    public Clan Clan { get; set; }
    //    public List<Stat> Stats { get; set; }
    //
    //          PlayerStats -> Clan
    //
    //  Clan
    //    public Info info { get; set; }
    //    public Member member { get; set; }
    //
    //          PlayerStats -> Clan -> Info
    //
    //  Info
    //    public string id { get; set; }
    //    public string name { get; set; }
    //    public string tag { get; set; }
    //    public int level { get; set; }
    //    public int levelPoints { get; set; }
    //    public DateTime registrationTime { get; set; }
    //    public string alliance { get; set; }
    //    public string description { get; set; }
    //    public string leader { get; set; }
    //    public int memberCount { get; set; }
    //
    //          PlayerStats -> Clan -> Member
    //
    //  Member
    //    public string name { get; set; }
    //    public string rank { get; set; }
    //    public DateTime joinTime { get; set; }
    //
    //          PlayerStats -> Stat
    //
    //  Stat
    //    public string id { get; set; }
    //    public string type { get; set; }
    //    public JToken value { get; set; }
}
