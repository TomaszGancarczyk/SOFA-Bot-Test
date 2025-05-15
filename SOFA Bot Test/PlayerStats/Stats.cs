namespace SOFA_Bot_Test.PlayerStats
{
    internal class Stats
    {
        internal string Uuid { get; set; }
        //image
        internal string Username { get; set; }
        internal string Faction { get; set; }
        internal string Clan { get; set; }
        internal string ClanRank { get; set; }
        internal string Playtime { get; set; }
        internal string JoinedGame { get; set; }
        internal string LastLogin { get; set; }
        internal string Kills { get; set; }
        internal string Deaths { get; set; }
        internal string Assists { get; set; }
        internal string Suicides { get; set; }
        internal string ArtifactsFound { get; set; }
        internal string HighestMoney { get; set; }
        internal string MoneyMade { get; set; }
        internal string BoltsThrown { get; set; }
        internal string MutantKills { get; set; }
        internal string NpcKills { get; set; }
        internal string DeliveriesMade { get; set; }
        internal string CachesFound { get; set; }
        internal string SignalsFound { get; set; }
    }






    //internal class PlayerStats
    //{
    //    public string Uuid { get; set; }
    //    public string Username { get; set; }
    //    public string Status { get; set; }
    //    public string Alliance { get; set; }
    //    public DateTime LastLogin { get; set; } //maybe string
    //    public List<string> DisplayedAchievements { get; set; }
    //    public Clan Clan { get; set; }
    //    public List<Stat> Stats { get; set; }
    //}
    ///// PlayerStats -> Clan
    //internal class Clan
    //{
    //    public Info info { get; set; }
    //    public Member member { get; set; }
    //}
    ///// PlayerStats -> Clan -> Info
    //internal class Info
    //{
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
    //}
    ///// PlayerStats -> Clan -> Member
    //internal class Member
    //{
    //    public string name { get; set; }
    //    public string rank { get; set; }
    //    public DateTime joinTime { get; set; }
    //}
    ///// PlayerStats -> Stat
    //internal class Stat
    //{
    //    public string id { get; set; }
    //    public string type { get; set; }
    //    public JToken value { get; set; }
    //}
}
