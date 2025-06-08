using Discord;
using Discord.WebSocket;
using FOFA_Bot.Attendance;


namespace FOFA_Bot
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

            _ = StartEvent();
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
        private async static Task StartEvent()
        {
            while (true)
            {
                if (BotHandler.GetCurrentMessageId() == null)
                {
                    await StartAttendanceEvent(false);
                    Task.Delay(7200000).Wait();
                }
                else
                    Task.Delay(60000).Wait();
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
            Logger.LogInformation($"/ Starting event");
            CurrentMessage = null;
            ulong? localCurrentMessageId = null;
            Logger.LogInformation($"Setting and getting event date time");
            await Attendance.Timer.SetEventDateTime(isToday);
            //DateTime eventDateTime = Attendance.Timer.GetEventDateTime();
            DateTime eventDateTime = DateTime.Now.AddMinutes(3);
            if (QuestionChannel != null && SignupsChannel != null)
            {
                IMessage? tempCurrentMessage = await MessageHandler.ValidateAndCreateMesage(QuestionChannel, SignupsChannel);
                if (tempCurrentMessage != null)
                {
                    CurrentMessage = tempCurrentMessage;
                    localCurrentMessageId = CurrentMessage.Id;
                }
                else
                {
                    Logger.LogError("CurrentMessage is null");
                    return;
                }
            }
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
            if (ValidateCurrentMessage(localCurrentMessageId))
            {
                TimeSpan reminderTimeSpan = eventDateTime - DateTime.Now.AddHours(1);
                if (reminderTimeSpan > TimeSpan.Zero)
                {
                    Logger.LogInformation($"Waiting for {reminderTimeSpan} for reminder");
                    Task.Delay(reminderTimeSpan).Wait();
                    Reminder.Handle();
                }
                else
                    Logger.LogWarning($"reminderTimeSpan is less than 0");
            }
            else return;
            if (ValidateCurrentMessage(localCurrentMessageId))
            {
                TimeSpan eventCloseTimeSpan = eventDateTime - DateTime.Now.AddMinutes(15);
                if (eventCloseTimeSpan > TimeSpan.Zero)
                {
                    Logger.LogInformation($"Waiting for {eventCloseTimeSpan} for closing event");
                    Task.Delay(eventCloseTimeSpan).Wait();
                }
                else
                    Logger.LogWarning($"eventCloseTimeSpan is less than 0");
                EmbedBuilder closedMessage = await SignupMessage.GetClosedSignupMessage();
                await CurrentMessage.Channel.ModifyMessageAsync(CurrentMessage.Id, message => message.Embed = closedMessage.Build());
            }
            else return;
            if (ValidateCurrentMessage(localCurrentMessageId))
            {
                CurrentMessage = null;
                Dictionary<SocketGuildUser, bool?> sofaMembersDict = await MemberHandler.GetSofaMembers();
                List<string>? sofaUnassignedMembers = [];
                foreach (var member in sofaMembersDict)
                {
                    if (member.Value == null) sofaUnassignedMembers.Add(member.Key.GlobalName);
                }
                if (sofaUnassignedMembers.Count > 0)
                    await AttendanceGoogleSheet.HandleUnsignedUsers(sofaUnassignedMembers);
                TimeSpan eventTimeSpan = eventDateTime - DateTime.Now;
                if (eventTimeSpan > TimeSpan.Zero)
                {
                    Logger.LogInformation($"Waiting for {eventTimeSpan} for finishing signup pipeline");
                    Task.Delay(eventTimeSpan).Wait();
                }
                else
                    Logger.LogError($"eventTimeSpan is less than 0");
            }
            else return;
            Logger.LogInformation("\\ Signup event finished");
        }

        private static bool ValidateCurrentMessage(ulong? localCurrentMessageId)
        {
            if (localCurrentMessageId != null && CurrentMessage != null && CurrentMessage.Id == localCurrentMessageId)
                return true;
            return false;
        }

        //TODO
        // handle player stats from API call
        // add people for reminder exceptions

        //TODO Known Bugs
        // /create-signup when waiting for question response new message may be created

        //TODO Testing
        // test handle a lot of people in one tab
        // test bot up for multiple days
        // test loging all people who didnt signed up to to google sheets
        //      test if file updates correctly
        //      test if sheet updates correctly
    }
}