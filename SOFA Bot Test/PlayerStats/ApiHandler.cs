using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SOFA_Bot_Test.PlayerStats
{
    internal class ApiHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Player Stats");
        internal async static Task<global::PlayerStats> GetPlayerStats(string playerName)
        {
            logger.LogInformation("{Time} - Calling API for {player}", DateTime.Now, playerName);
            string apitoken = BotInfo.GetApiToken();

            HttpClient client = new()
            {
                BaseAddress = new Uri("https://eapi.stalcraft.net/eu/character/by-name/")
            };
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apitoken);
            client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync($"{playerName}/profile").Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                logger.LogInformation("{Time} - Successfully got the request for {player}", DateTime.Now, playerName);
                string responseBody = await response.Content.ReadAsStringAsync();
                PlayerStats playerStats = JsonSerializer.Deserialize<PlayerStats>(responseBody, options);
                return playerStats;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                logger.LogCritical("{Time} - Unauthorized API response", DateTime.Now);
                return null;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                logger.LogWarning("{Time} - Couldnt find player {player}", DateTime.Now, playerName);
                return null;
            }
            else
            {
                logger.LogWarning("{Time} - Unsupported error in API call: {error code}", DateTime.Now, response.StatusCode);
                return null;
            }
        }
    }
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