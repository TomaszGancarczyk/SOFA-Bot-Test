using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFA_Bot_Test.Stats
{
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    internal class PlayerStats
    {
        public string username { get; set; }
        public string uuid { get; set; }
        public string status { get; set; }
        public string alliance { get; set; }
        public DateTime lastLogin { get; set; } //maybe string
        public List<string> displayedAchievements { get; set; }
        public Clan clan { get; set; }
        public List<Stat> stats { get; set; }
    }
    /// PlayerStats -> Clan
    internal class Clan
    {
        public Info info { get; set; }
        public Member member { get; set; }
    }
    /// PlayerStats -> Clan -> Info
    internal class Info
    {
        public string id { get; set; }
        public string name { get; set; }
        public string tag { get; set; }
        public int level { get; set; }
        public int levelPoints { get; set; }
        public DateTime registrationTime { get; set; }
        public string alliance { get; set; }
        public string description { get; set; }
        public string leader { get; set; }
        public int memberCount { get; set; }
    }
    /// PlayerStats -> Clan -> Member
    internal class Member
    {
        public string name { get; set; }
        public string rank { get; set; }
        public DateTime joinTime { get; set; }
    }
    /// PlayerStats -> Stat
    internal class Stat
    {
        public string id { get; set; }
        public string type { get; set; }
        public JToken value { get; set; }
    }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore CA1050 // Declare types in namespaces
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}
