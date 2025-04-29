using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace SOFA_Bot_Test.PlayerStats
{
    internal class ApiHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Player Stats");
        internal async static Task<PlayerStatsDeserialized> GetPlayerStats(string playerName)
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
                logger.LogInformation("{Time} - Successfully got the request for {player}", DateTime.Now, playerName);
                string responseBody = await response.Content.ReadAsStringAsync();
                PlayerStatsDeserialized playerStats = JsonConvert.DeserializeObject<PlayerStatsDeserialized>(responseBody);
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
}

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public class Clan
{
    public Info info { get; set; }
    public Member member { get; set; }
}

public class Info
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

public class Member
{
    public string name { get; set; }
    public string rank { get; set; }
    public DateTime joinTime { get; set; }
}

public class PlayerStatsDeserialized
{
    public string username { get; set; }
    public string uuid { get; set; }
    public string status { get; set; }
    public string alliance { get; set; }
    public DateTime lastLogin { get; set; }
    public List<string> displayedAchievements { get; set; }
    public Clan clan { get; set; }
    public List<Stat> stats { get; set; }
}

public class Stat
{
    public string id { get; set; }
    public string type { get; set; }
    public Value value { get; set; }
}

public class Value
{
}
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore CA1050 // Declare types in namespaces
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.