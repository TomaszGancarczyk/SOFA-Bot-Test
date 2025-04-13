using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class BotHandler
    {
        private static DiscordSocketClient Discord;
        private static SocketGuild Guild;
        private static IMessageChannel CanWarChannel;
        private static IMessageChannel GoldenDropChannel;
        private const string SofaRoleName = "SOFA";
        private const string RofaRoleName = "ROFA";
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");

        internal static void InitializeBotHandler(DiscordSocketClient discord)
        {
            Discord = discord;
            Discord.ButtonExecuted += ButtonHandler.Handler;
            Guild = discord.GetGuild(BotInfo.GetGuildId());
            if (Guild == null)
            {
                logger.LogError("{Time} - Guild not found", DateTime.Now);
                throw new ArgumentNullException(nameof(Guild));
            }
            logger.LogInformation("{Time} - Found Guild: {Guild.Name}", DateTime.Now, Guild.Name);

            CanWarChannel = (IMessageChannel)Guild.GetChannel(BotInfo.GetCWChannelId());
            if (CanWarChannel == null)
            {
                logger.LogError("{Time} - Channel not found", DateTime.Now);
                throw new ArgumentNullException(nameof(CanWarChannel));
            }
            logger.LogInformation("{Time} - Found CW Channel: {Channel.Name}", DateTime.Now, CanWarChannel.Name);

            GoldenDropChannel = (IMessageChannel)Guild.GetChannel(BotInfo.GetGoldenDropChannelId());
            if (GoldenDropChannel == null)
            {
                logger.LogError("{Time} - Channel not found", DateTime.Now);
                throw new ArgumentNullException(nameof(GoldenDropChannel));
            }
            logger.LogInformation("{Time} - Found Golden Drop Channel: {Channel.Name}", DateTime.Now, GoldenDropChannel.Name);

            StartEvent();
        }

        internal static SocketRole GetRole(string roleName)
        {
            return Guild.Roles.FirstOrDefault(role => role.Name == roleName);
        }
        internal static SocketGuildUser[] GetSofaMembers()
        {
            SocketRole role = Guild.Roles.FirstOrDefault(role => role.Name == SofaRoleName);
            SocketGuildUser[] userList = Guild.Users.Where(user => user.Roles.Contains(role)).ToArray();
            return userList;
        }
        internal static SocketGuildUser[] GetRofaMembers()
        {
            SocketRole role = Guild.Roles.FirstOrDefault(role => role.Name == RofaRoleName);
            SocketGuildUser[] userList = Guild.Users.Where(user => user.Roles.Contains(role)).ToArray();
            return userList;
        }
        internal static DiscordSocketClient GetDiscord()
        {
            return Discord;
        }
        private async static void StartEvent()
        {
            logger.LogInformation("{Time} - Starting event", DateTime.Now);
            logger.LogInformation("{Time} - Getting event date time", DateTime.Now);
            Timer.SetEventDateTimeForNextDay();
            DateTime eventDateTime = Timer.GetEventDateTime();
            logger.LogInformation("{Time} - Event date time set for {eventDateTime}", DateTime.Now, eventDateTime);
            IMessage eventMessage = await MessageHandler.CreateMesage(CanWarChannel, GoldenDropChannel, eventDateTime.DayOfWeek);
            //TODO Continue after message is sent
            //await eventMessage.AddReactionAsync(new Emoji("⚫"));
        }
    }
}