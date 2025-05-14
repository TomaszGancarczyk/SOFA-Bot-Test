using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace SOFA_Bot_Test.PlayerStats
{
    internal class ApiHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Player Stats");
        internal static async Task<Stats> GetPlayerStats(string playerName)
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
                Stats playerStats = await ConvertJsonStringToPlayerStats(responseBody);
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
        private static async Task<Stats> ConvertJsonStringToPlayerStats(string jsonString)
        {
            dynamic dynamicStats = JsonConvert.DeserializeObject(jsonString);
            Stats stats = new()
            {
                Uuid = dynamicStats.uuid,
                Username = dynamicStats.username
            };
            return stats;
        }
    }
}