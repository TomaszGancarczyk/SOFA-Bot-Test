using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class Program
    {
        private readonly DiscordSocketClient discord;
        private static readonly string Token = BotInfo.GetToken();
        private static readonly SocketGuild Guild = BotInfo.GetGuild();
        private static readonly IMessageChannel Channel = BotInfo.GetChannel(Guild);
        private SocketGuildUser[] Users = BotInfo.GetMembers(Guild);
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");
        static Task Main()
        {
            logger.LogInformation("{DateTime.Now} - [SOFA] Signups Bot is starting", DateTime.Now);
            new Program().StartBotAsync().GetAwaiter().GetResult();
            return Task.CompletedTask;
        }
        public async Task StartBotAsync()
        {
            await discord.LoginAsync(TokenType.Bot, Token);
            await discord.StartAsync();
            discord.Ready += () =>
            {
                logger.LogInformation("{DateTime.Now} - [SOFA] Signups Bot is running", DateTime.Now);
                //new CommandHandler().FirstCommandHandler(discord, DateTime.Now.Hour, DateTime.Now.Minute + 1);
                return Task.CompletedTask;
            };
            await Task.Delay(-1);
        }
    }
}