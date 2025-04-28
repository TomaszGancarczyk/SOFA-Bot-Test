using System.IO;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SOFA_Bot_Test.PlayerStats
{
    internal class ApiHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Player Stats");
        internal async static Task<string> GetPlayerStats(string playerName)
        {
            logger.LogInformation("{Time} - Calling API for {player}", DateTime.Now, playerName);
            string apitoken = BotInfo.GetApiToken();

            HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("https://dapi.stalcraft.net/EU/character/by-name/");
            client.BaseAddress = new Uri("https://dapi.stalcraft.net/EU/characters");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apitoken);
            client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //HttpResponseMessage response = client.GetAsync($"{playerName}/profile").Result;
            HttpResponseMessage response = client.GetAsync($"").Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                logger.LogInformation("{Time} - Successfully got the request for {player}", DateTime.Now, playerName);
                return "e";
            }
            else
            {
                logger.LogWarning("{Time} - Couldnt find player {player}", DateTime.Now, playerName);
                return null;
            }
        }
    }
}
