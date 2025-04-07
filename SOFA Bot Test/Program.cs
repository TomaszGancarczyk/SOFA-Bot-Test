using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class Program
    {
        private readonly DiscordSocketClient Discord;
        private static readonly string Token = BotInfo.GetToken();
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");

        static Task Main()
        {
            logger.LogInformation("{Time} - [SOFA] Signups Bot is starting", DateTime.Now);
            new Program().StartBotAsync().GetAwaiter().GetResult();
            return Task.CompletedTask;
        }

        internal Program()
        {
            var config = new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 100,
                GatewayIntents = GatewayIntents.All
            };
            Discord = new DiscordSocketClient(config);
        }

        internal async Task StartBotAsync()
        {
            await Discord.LoginAsync(TokenType.Bot, Token);
            await Discord.StartAsync();
            Discord.Ready += () =>
            {
                logger.LogInformation("{Time} - [SOFA] Signups Bot is running", DateTime.Now);
                return Task.CompletedTask;
            };
            await Task.Delay(3000);
            BotHandler.InitializeBotHandler(Discord);
            await Task.Delay(-1);
        }
    }
}