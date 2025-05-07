using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SOFA_Bot_Test.Attendance;

namespace SOFA_Bot_Test
{
    internal class BotHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");
        private static SocketGuild Guild;
        private static IMessageChannel QuestionChannel;
        private static IMessageChannel SignupsChannel;
        private static IMessageChannel GoldenDropChannel;
        private static IMessage? CurrentMessage;

        internal static void InitializeBotHandler(DiscordSocketClient discord)
        {
            Guild = discord.GetGuild(BotInfo.GetGuildId());
            if (Guild == null)
            {
                logger.LogCritical("{Time} - Guild not found", DateTime.Now);
                throw new ArgumentNullException("Guild not found");
            }
            logger.LogInformation("{Time} - Found Guild: {Guild.Name}", DateTime.Now, Guild.Name);

            QuestionChannel = (IMessageChannel)Guild.GetChannel(BotInfo.GetQuestionChannelId());
            if (QuestionChannel == null)
            {
                logger.LogCritical("{Time} - Quesion channel not found", DateTime.Now);
                throw new ArgumentNullException("Question channel not found");
            }
            logger.LogInformation("{Time} - Found Question Channel: {Channel.Name}", DateTime.Now, QuestionChannel.Name);

            SignupsChannel = (IMessageChannel)Guild.GetChannel(BotInfo.GetSignupsChannelId());
            if (SignupsChannel == null)
            {
                logger.LogCritical("{Time} - Signups channel not found", DateTime.Now);
                throw new ArgumentNullException("Signups channel not found");
            }
            logger.LogInformation("{Time} - Found Signups Channel: {Channel.Name}", DateTime.Now, SignupsChannel.Name);

            GoldenDropChannel = (IMessageChannel)Guild.GetChannel(BotInfo.GetGoldenDropChannelId());
            if (GoldenDropChannel == null)
            {
                logger.LogCritical("{Time} - Golden Drop channel not found", DateTime.Now);
                throw new ArgumentNullException("Golden Drop channel not found");
            }
            logger.LogInformation("{Time} - Found Golden Drop Channel: {Channel.Name}", DateTime.Now, GoldenDropChannel.Name);

            StartEvent();
        }

        internal static SocketRole GetRole(string roleName)
        {
            return Guild.Roles.FirstOrDefault(role => role.Name == roleName);
        }
        internal static SocketGuild GetGuild()
        {
            return Guild;
        }
        internal static void SetCurrentMessage(IMessage message)
        {
            CurrentMessage = message;
        }
        internal static ulong GetCurrentMessageId()
        {
            return CurrentMessage.Id;
        }
        private async static void StartEvent()
        {
            while (true)
            {
                await HandleEvent();
            }
        }
        internal async static Task<List<SocketRole>> GetPrivilegedRoles()
        {
            List<SocketRole> privilegedRoles = null;
            string[] roleNames = BotInfo.GetPrivilegedRoleNames();
            foreach (string roleName in roleNames)
            {
                SocketRole role = Guild.Roles.FirstOrDefault(role => role.Name == roleName);
                if (role != null)
                {
                    privilegedRoles.Add(role);
                }
            }
            return privilegedRoles;
        }
        internal async static Task<SocketGuildUser> GetGuildUserByName(string userName)
        {
            return Guild.Users.FirstOrDefault(user => user.GlobalName == userName);
        }
        internal async static Task HandleEvent()
        {
            logger.LogInformation("{Time} - Starting event", DateTime.Now);
            CurrentMessage = null;
            logger.LogInformation("{Time} - Getting event date time", DateTime.Now);
            Timer.SetEventDateTimeForNextDay();
            var eventDateTime = Timer.GetEventDateTime();
            CurrentMessage = await MessageHandler.CreateMesage(QuestionChannel, SignupsChannel, GoldenDropChannel);
            TimeSpan reminderTimeSpan = eventDateTime - DateTime.Now.AddHours(1);
            if (reminderTimeSpan > TimeSpan.Zero)
                Reminder.Handle(reminderTimeSpan);
            else
                logger.LogWarning("{Time} - reminderTimeSpan is less than 0", DateTime.Now);
            TimeSpan eventCloseTimeSpan = eventDateTime - DateTime.Now.AddMinutes(20);
            if (eventCloseTimeSpan > TimeSpan.Zero)
                Task.Delay(eventCloseTimeSpan).Wait();
            else
                logger.LogWarning("{Time} - eventCloseTimeSpan is less than 0", DateTime.Now);
            EmbedBuilder closedMessage = await CreateMessage.CloseAttendanceMessage();
            await CurrentMessage.Channel.ModifyMessageAsync(CurrentMessage.Id, message => message.Embed = closedMessage.Build());
            CurrentMessage = null;
            Task.Delay(7200000).Wait();
        }
        //TODO check if question message is the right one

        //TODO handle player stats from API call

        //TODO Testing
        // test people changing roles mid signup
        // test people with multiple roles handled correctly
        // test spamming signup button
        // test handle a lot of people in one tab
        // test /reminder
        // test /createsignup
        // test slash command permissions
        // test reminder messages sent on correct days
    }
}