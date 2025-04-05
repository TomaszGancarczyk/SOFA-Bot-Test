using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class BotHandler
    {
        private static SocketGuild Guild;
        private static IMessageChannel Channel;
        private static SocketGuildUser[] RoleUsers;
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");

        public static void InitializeBotHandler(DiscordSocketClient discord)
        {
            try
            {
                Guild = discord.GetGuild(BotInfo.GetGuildId());
                logger.LogInformation("{Time} - Found Guild: {Guild.Name}", DateTime.Now, Guild.Name);
            }
            catch
            {
                logger.LogError("{Time} - Guild not found", DateTime.Now);
            }
            try
            {
                Channel = (IMessageChannel)Guild.GetChannel(BotInfo.GetChannelId());
                logger.LogInformation("{Time} - Found Channel: {Channel.Name}", DateTime.Now, Channel.Name);
            }
            catch
            {
                logger.LogError("{Time} - Channel not found", DateTime.Now);
            }
            RoleUsers = BotInfo.GetMembers(Guild);
            if (RoleUsers.Count() > 0)
            {
                logger.LogInformation("{Time} - Found {Number} users with proper role", DateTime.Now, RoleUsers.Count());
            }
            else
            {
                logger.LogError("{Time} - No users found with proper role", DateTime.Now);
            }
        }
    }
}