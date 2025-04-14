using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class BotHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");
        private static DiscordSocketClient Discord;
        private static SocketGuild Guild;
        private static IMessageChannel QuestionChannel;
        private static IMessageChannel SignupsChannel;
        private static IMessageChannel GoldenDropChannel;
        private const string SofaRoleName = "SOFA";
        private static Dictionary<SocketGuildUser, bool?> SofaMembers;
        private const string RofaRoleName = "ROFA";
        private static Dictionary<SocketGuildUser, bool?> RofaMembers;

        internal static void InitializeBotHandler(DiscordSocketClient discord)
        {
            Discord = discord;
            Discord.ButtonExecuted += ButtonEventHandler.Handler;
            Guild = discord.GetGuild(BotInfo.GetGuildId());
            if (Guild == null)
            {
                logger.LogError("{Time} - Guild not found", DateTime.Now);
                throw new ArgumentNullException(nameof(Guild));
            }
            logger.LogInformation("{Time} - Found Guild: {Guild.Name}", DateTime.Now, Guild.Name);

            QuestionChannel = (IMessageChannel)Guild.GetChannel(BotInfo.GetQuestionChannelId());
            if (QuestionChannel == null)
            {
                logger.LogError("{Time} - Quesion channel not found", DateTime.Now);
                throw new ArgumentNullException(nameof(QuestionChannel));
            }
            logger.LogInformation("{Time} - Found Question Channel: {Channel.Name}", DateTime.Now, QuestionChannel.Name);

            SignupsChannel = (IMessageChannel)Guild.GetChannel(BotInfo.GetSignupsChannelId());
            if (SignupsChannel == null)
            {
                logger.LogError("{Time} - Signups channel not found", DateTime.Now);
                throw new ArgumentNullException(nameof(SignupsChannel));
            }
            logger.LogInformation("{Time} - Found Signups Channel: {Channel.Name}", DateTime.Now, SignupsChannel.Name);

            GoldenDropChannel = (IMessageChannel)Guild.GetChannel(BotInfo.GetGoldenDropChannelId());
            if (GoldenDropChannel == null)
            {
                logger.LogError("{Time} - Golden Drop channel not found", DateTime.Now);
                throw new ArgumentNullException(nameof(GoldenDropChannel));
            }
            logger.LogInformation("{Time} - Found Golden Drop Channel: {Channel.Name}", DateTime.Now, GoldenDropChannel.Name);

            StartEvent();
        }

        internal static SocketRole GetRole(string roleName)
        {
            return Guild.Roles.FirstOrDefault(role => role.Name == roleName);
        }
        internal static void SetSofaMembers()
        {
            SocketRole role = Guild.Roles.FirstOrDefault(role => role.Name == SofaRoleName);
            SocketGuildUser[] sofaMembers = Guild.Users.Where(user => user.Roles.Contains(role)).ToArray();
            foreach (SocketGuildUser member in sofaMembers)
            {
                SofaMembers.Add(member, null);
            }
        }
        internal static Dictionary<SocketGuildUser, bool?> GetSofaMembers()
        {
            return SofaMembers;
        }
        internal static void SetRofaMembers()
        {
            SocketRole role = Guild.Roles.FirstOrDefault(role => role.Name == RofaRoleName);
            SocketGuildUser[] rofaMembers = Guild.Users.Where(user => user.Roles.Contains(role)).ToArray();
            foreach (SocketGuildUser member in rofaMembers)
            {
                RofaMembers.Add(member, null);
            }
        }
        internal static Dictionary<SocketGuildUser, bool?> GetRofaMembers()
        {
            return RofaMembers;
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
            IMessage eventMessage = await MessageHandler.CreateMesage(QuestionChannel, SignupsChannel, GoldenDropChannel, eventDateTime.DayOfWeek);
            //TODO Continue after message is sent
            //await eventMessage.AddReactionAsync(new Emoji("⚫"));
        }
    }
}