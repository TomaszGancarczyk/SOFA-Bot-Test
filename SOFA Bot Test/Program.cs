using Discord;
using Discord.WebSocket;
using SOFA_Bot_Test.Attendance;


namespace SOFA_Bot_Test
{
    internal class Program
    {
        private readonly DiscordSocketClient Discord;
        private static readonly string Token = BotInfo.GetDiscordToken();
        static Task Main()
        {
            Logger.LogInformation($"[FOFA] Bot is starting");
            new Program().StartBotAsync().GetAwaiter().GetResult();
            return Task.CompletedTask;
        }

        internal Program()
        {
            DiscordSocketConfig? config = new DiscordSocketConfig
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
                Logger.LogInformation($"[FOFA] Bot is running");
                return Task.CompletedTask;
            };
            await Task.Delay(3000);
            BotHandler.InitializeBotHandler(Discord);
            await Task.Delay(-1);
        }
        private async Task DiscordReady()
        {
            SlashCommandBuilder? statsCommand = new Discord.SlashCommandBuilder()
                .WithName("stats")
                .WithDescription("Lists player's stalcraft stats")
                .AddOption("player", ApplicationCommandOptionType.String, "The name of a player whos stats you want to see", isRequired: true);
            SlashCommandBuilder? reminderMessageCommand = new Discord.SlashCommandBuilder()
                .WithName("reminder-message")
                .WithDescription("Sets status of reminder messages, true = messages are sent")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("status")
                    .WithDescription("Do you want the bot to send reminder messages?")
                    .WithRequired(true)
                    .AddChoice("Yes", 1)
                    .AddChoice("No", 0)
                    .WithType(ApplicationCommandOptionType.Integer)
                );
            SlashCommandBuilder? createSignupCommand = new Discord.SlashCommandBuilder()
                .WithName("create-signup")
                .WithDescription("Create new signup for next day")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("when")
                    .WithDescription("Is is signup for today or tomorrow?")
                    .WithRequired(true)
                    .AddChoice("Today", 1)
                    .AddChoice("Tomorrow", 0)
                    .WithType(ApplicationCommandOptionType.Integer)
                );
            try
            {
                await Discord.CreateGlobalApplicationCommandAsync(createSignupCommand.Build());
                await Discord.CreateGlobalApplicationCommandAsync(statsCommand.Build());
                await Discord.CreateGlobalApplicationCommandAsync(reminderMessageCommand.Build());
            }
            catch (Exception e)
            {
                Logger.LogCritical($"{e}");
            }
        }
    }
}