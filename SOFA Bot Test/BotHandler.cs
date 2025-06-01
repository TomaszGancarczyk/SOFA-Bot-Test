using Discord;
using Discord.WebSocket;
using SOFA_Bot_Test.Attendance;

namespace SOFA_Bot_Test
{
    internal class BotHandler
    {
        private static SocketGuild Guild;
        private static IMessageChannel QuestionChannel;
        private static IMessageChannel SignupsChannel;
        private static IMessage? CurrentMessage;

        internal static void InitializeBotHandler(DiscordSocketClient discord)
        {
            Guild = discord.GetGuild(BotInfo.GetGuildId());
            if (Guild == null)
            {
                Logger.LogCritical($"Guild not found");
                throw new ArgumentException("Guild not found");
            }
            Logger.LogInformation($"Found Guild: {Guild.Name}");

            QuestionChannel = (IMessageChannel)Guild.GetChannel(BotInfo.GetQuestionChannelId());
            if (QuestionChannel == null)
            {
                Logger.LogCritical($"Quesion channel not found");
                throw new ArgumentException("Question channel not found");
            }
            Logger.LogInformation($"Found Question Channel: {QuestionChannel.Name}");

            SignupsChannel = (IMessageChannel)Guild.GetChannel(BotInfo.GetSignupsChannelId());
            if (SignupsChannel == null)
            {
                Logger.LogCritical($"Signups channel not found");
                throw new ArgumentException("Signups channel not found");
            }
            Logger.LogInformation($"Found Signups Channel: {SignupsChannel.Name}");

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
        internal static ulong? GetCurrentMessageId()
        {
            if (CurrentMessage == null) return null;
            else return CurrentMessage.Id;
        }
        internal static void SetCurrentMessage(IMessage message)
        {
            CurrentMessage = message;
        }
        internal static ulong? GetSignupsChannelId()
        {
            if (SignupsChannel == null) return null;
            else return SignupsChannel.Id;
        }
        private async static void StartEvent()
        {
            while (true)
            {
                await StartAttendanceEvent(false);
            }
        }
        internal async static Task<SocketGuildUser> GetGuildUserByName(string userName)
        {
            return Guild.Users.FirstOrDefault(user => user.GlobalName == userName);
        }
        internal async static Task StartAttendanceEvent(bool isToday)
        {
            Logger.LogInformation($"Starting event");
            CurrentMessage = null;
            Logger.LogInformation($"Getting event date time");
            Attendance.Timer.SetEventDateTimeForNextDay(isToday);
            var eventDateTime = Attendance.Timer.GetEventDateTime();
            CurrentMessage = await MessageHandler.CreateMesage(QuestionChannel, SignupsChannel);
            TimeSpan reminderTimeSpan = eventDateTime - DateTime.Now.AddHours(1);
            if (reminderTimeSpan > TimeSpan.Zero)
            {
                Task.Delay(reminderTimeSpan).Wait();
                Reminder.Handle();
            }
            else
                Logger.LogWarning($"reminderTimeSpan is less than 0");
            TimeSpan eventCloseTimeSpan = eventDateTime - DateTime.Now.AddMinutes(15);
            if (eventCloseTimeSpan > TimeSpan.Zero)
                Task.Delay(eventCloseTimeSpan).Wait();
            else
                Logger.LogWarning($"eventCloseTimeSpan is less than 0");
            EmbedBuilder closedMessage = await SignupMessage.CloseSignupMessage();
            await CurrentMessage.Channel.ModifyMessageAsync(CurrentMessage.Id, message => message.Embed = closedMessage.Build());
            CurrentMessage = null;
            Task.Delay(7200000).Wait();
        }

        //TODO
        // handle player stats from API call
        // expand signup reminder message?
        //      check if gif, signup link, reminder message works
        // expand signup response message?

        //TODO Testing
        //
        // test handle a lot of people in one tab
        // test people changing roles mid signup
        // --test spamming question button--
        // --test spamming signup button--
        // test /reminder
        //      test functionality
        //          if its sent for correct events
        //          --enabling/disabling is working--
        //      --permissions are working--
        //      --response is working--
        // test if there are no visual bugs for messages
    }
}