using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class BotHandler
    {
        private static SocketGuild Guild;
        private static IMessageChannel clanWarChannel;
        private static IMessageChannel GoldenDropChannel;
        private static SocketGuildUser[] RoleUsers;
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");

        internal static void InitializeBotHandler(DiscordSocketClient discord)
        {
            Guild = discord.GetGuild(BotInfo.GetGuildId());
            if (Guild == null)
            {
                logger.LogError("{Time} - Guild not found", DateTime.Now);
                throw new ArgumentNullException(nameof(Guild));
            }
            logger.LogInformation("{Time} - Found Guild: {Guild.Name}", DateTime.Now, Guild.Name);

            clanWarChannel = (IMessageChannel)Guild.GetChannel(BotInfo.GetCWChannelId());
            if (clanWarChannel == null)
            {
                logger.LogError("{Time} - Channel not found", DateTime.Now);
                throw new ArgumentNullException(nameof(clanWarChannel));
            }
            logger.LogInformation("{Time} - Found CW Channel: {Channel.Name}", DateTime.Now, clanWarChannel.Name);

            GoldenDropChannel = (IMessageChannel)Guild.GetChannel(BotInfo.GetGoldenDropChannelId());
            if (GoldenDropChannel == null)
            {
                logger.LogError("{Time} - Channel not found", DateTime.Now);
                throw new ArgumentNullException(nameof(GoldenDropChannel));
            }
            logger.LogInformation("{Time} - Found Golden Drop Channel: {Channel.Name}", DateTime.Now, GoldenDropChannel.Name);
            StartEvent();
        }
        private async static void StartEvent()
        {
            logger.LogInformation("{Time} - Starting event", DateTime.Now);
            RoleUsers = BotInfo.GetMembers(Guild);
            if (RoleUsers.Length > 0)
            {
                logger.LogInformation("{Time} - Found {Number} users with proper role", DateTime.Now, RoleUsers.Length);
            }
            else
            {
                logger.LogWarning("{Time} - No users found with proper role", DateTime.Now);
            }
            logger.LogInformation("{Time} - Getting event date time", DateTime.Now);
            Timer.SetEventDateTimeForNextDay();
            DateTime eventDateTime = Timer.GetEventDateTime();
            logger.LogInformation("{Time} - Event date time set for {eventDateTime}", DateTime.Now, eventDateTime);
            IMessage eventMessage = await MessageHandler.CreateMesage(clanWarChannel, GoldenDropChannel, eventDateTime.DayOfWeek);
            //TODO Continue after message is sent
        }
    }
}