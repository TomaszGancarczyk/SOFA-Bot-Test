using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SOFA_Bot_Test
{
    internal class ApiHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Player Stats");
        internal static async Task<Stats.PlayerStats> GetPlayerStats(string playerName)
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
                Stats.PlayerStats playerStats = JsonSerializer.Deserialize<Stats.PlayerStats>(responseBody, options);
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