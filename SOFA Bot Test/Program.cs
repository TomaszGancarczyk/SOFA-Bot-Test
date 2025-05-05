using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SOFA_Bot_Test.Attendance;

namespace SOFA_Bot_Test
{
    internal class Program
    {
        private readonly DiscordSocketClient Discord;
        private static readonly string Token = BotInfo.GetDiscordToken();
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
                GatewayIntents = GatewayIntents.All,
                UseInteractionSnowflakeDate = false
            };
            Discord = new DiscordSocketClient(config);
        }

        internal async Task StartBotAsync()
        {
            await Discord.LoginAsync(TokenType.Bot, Token);
            await Discord.StartAsync();
            Discord.Ready += DiscordReady;
            Discord.Ready += () =>
            {
                Discord.ButtonExecuted += ButtonEventHandler.Handler;
                Discord.SlashCommandExecuted += SlashCommandHandler.Handler;
                logger.LogInformation("{Time} - [SOFA] Signups Bot is running", DateTime.Now);
                return Task.CompletedTask;
            };
            await Task.Delay(3000);
            BotHandler.InitializeBotHandler(Discord);
            await Task.Delay(-1);
        }
        private async Task DiscordReady()
        {
            var statsCommand = new Discord.SlashCommandBuilder()
                .WithName("stats")
                .WithDescription("Lists player's stalcraft stats")
                .AddOption("player", ApplicationCommandOptionType.String, "The name of a player whos stats you want to see", isRequired: true);
            var reminderMessageCommand = new Discord.SlashCommandBuilder()
                .WithName("reminderMessage")
                .WithDescription("Sets status of reminder messages, true = messages are sent")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("status")
                    .WithDescription("Do you want the bot to send reminder messages?")
                    .WithRequired(true)
                    .AddChoice("Yes", 1)
                    .AddChoice("No", 0)
                    .WithType(ApplicationCommandOptionType.Integer)
        );
            try
            {
                await Discord.Rest.CreateGlobalCommand(statsCommand.Build());
                await Discord.Rest.CreateGlobalCommand(reminderMessageCommand.Build());
            }
            catch (Exception e)
            {
                logger.LogCritical("{error}", e.ToString());
            }
        }
    }
}