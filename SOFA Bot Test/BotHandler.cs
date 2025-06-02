using Discord;
using Discord.WebSocket;
using SOFA_Bot_Test.Attendance;

namespace SOFA_Bot_Test
{
    internal class BotHandler
    {
        private static SocketGuild? Guild;
        private static IMessageChannel? QuestionChannel;
        private static IMessageChannel? SignupsChannel;
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
            if (Guild != null)
            {
                SocketRole? role = Guild.Roles.FirstOrDefault(role => role.Name == roleName);
                if (role != null)
                    return role;
                else
                {
                    Logger.LogError($"Cannot get role with name {roleName}");
                    return null;
                }
            }
            else
            {
                Logger.LogCritical($"Cannot get Guild");
                return null;
            }
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
            if (Guild != null)
            {
                SocketGuildUser? user = Guild.Users.FirstOrDefault(user => user.GlobalName == userName);
                if (user != null)
                    return user;
                else
                {
                    Logger.LogError($"Cannot find user with the name {userName}");
                    return null;
                }
            }
            else
            {
                Logger.LogCritical($"Cannot get Guild");
                return null;
            }
        }
        internal async static Task StartAttendanceEvent(bool isToday)
        {
            Logger.LogInformation($"Starting event");
            CurrentMessage = null;
            Logger.LogInformation($"Getting event date time");
            Attendance.Timer.SetEventDateTimeForNextDay(isToday);
            var eventDateTime = Attendance.Timer.GetEventDateTime();
            if (QuestionChannel != null && SignupsChannel != null)
                CurrentMessage = await MessageHandler.CreateMesage(QuestionChannel, SignupsChannel);
            else
            {
                if (QuestionChannel == null)
                {
                    Logger.LogError("QuestionChannel is null");
                    return;
                }
                if (SignupsChannel == null)
                {
                    Logger.LogError("SignupsChannel is null");
                    return;
                }
            }
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
            if (CurrentMessage != null)
                await CurrentMessage.Channel.ModifyMessageAsync(CurrentMessage.Id, message => message.Embed = closedMessage.Build());
            else
                Logger.LogError("CurrentMessage is null for signup message");
            CurrentMessage = null;
            Task.Delay(7200000).Wait();
        }

        //TODO
        // handle player stats from API call
        // log all people who didnt signed up to file to export to excel

        //TODO Testing
        // test handle a lot of people in one tab
    }
}