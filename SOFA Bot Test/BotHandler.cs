using Discord;
using Discord.WebSocket;
using FOFA_Bot.Attendance;
using FOFA_Bot.Nades;


namespace FOFA_Bot
{
    internal class BotHandler
    {
        private static SocketGuild? Guild;
        private static IMessageChannel? QuestionChannel;
        private static IMessageChannel? SignupsChannel;
        private static IMessageChannel? NadeChannel;
        private static IMessage? CurrentMessage;
        private static bool isDayOffRunning;

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

            NadeChannel = (IMessageChannel)Guild.GetChannel(BotInfo.GetNadeChannelId());
            if (NadeChannel == null)
            {
                Logger.LogCritical($"Nade channel not found");
                throw new ArgumentException("Nade channel not found");
            }
            Logger.LogInformation($"Found Nade Channel: {NadeChannel.Name}");

            _ = StartEvent();
        }
        private static async Task StartEvent()
        {
            await StartAttendanceEvent(false);
            while (true)
            {
                while (CurrentMessage.Id != null || isDayOffRunning == true)
                    Task.Delay(60000).Wait();
                Logger.LogInformation("There is no active event, running cooldown for new event");
                Task.Delay(7200000).Wait();
                if (CurrentMessage.Id == null)
                    await StartAttendanceEvent(false);
            }
        }

        internal static SocketRole? GetRole(string roleName)
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
        internal static SocketGuild? GetGuild()
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
        internal static async Task<SocketGuildUser?> GetGuildUserByName(string userName)
        {
            if (Guild != null)
            {
                SocketGuildUser? user = Guild.Users.FirstOrDefault(user => user.Username == userName);
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

        internal static async Task StartAttendanceEvent(bool isToday)
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                _ = NadeHandler.StartNadeEvent();
            Logger.LogInformation($"/ Starting event");
            CurrentMessage = null;
            ulong? localCurrentMessageId = null;
            Logger.LogInformation($"Setting and getting event date time");
            await Attendance.Timer.SetEventDateTime(isToday);
            DateTime eventDateTime = Attendance.Timer.GetEventDateTime();
            if (QuestionChannel != null && SignupsChannel != null)
            {
                IMessage? tempCurrentMessage = await AttendanceMessageHandler.ValidateAndCreateMesage(QuestionChannel, SignupsChannel, isToday);
                if (tempCurrentMessage != null)
                {
                    if (eventDateTime > DateTime.Now)
                        Logger.LogInformation($"Event date time set for {eventDateTime}");
                    CurrentMessage = tempCurrentMessage;
                    localCurrentMessageId = CurrentMessage.Id;
                }
                else
                {
                    Logger.LogInformation("Handling DayOff");
                    CurrentMessage = null;
                    isDayOffRunning = true;
                    TimeSpan reminderTimeSpan = eventDateTime - DateTime.Now;
                    Task.Delay(reminderTimeSpan).Wait();
                    isDayOffRunning = false;
                    Logger.LogInformation("Finished DayOff");
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
                if (CurrentMessage != null)
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
                    if (member.Value == null) sofaUnassignedMembers.Add(member.Key.Username);
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
        internal static IMessageChannel? GetNadeChannel()
        {
            if (NadeChannel != null)
                return NadeChannel;
            else
                return null;
        }
        //TODO Task list
        // move data to files to restore it when power dies
        // add people for reminder exceptions
        // handle player stats from API call


        //TODO Known Bugs
        // /create-signup when waiting for question response new message may be created

        //TODO Verify
        // absent button sometimes may not respond with leave channel reminder
        // DayOff colldown works properly
        // base cap eventDateTime is 1 hour earlier
        // remove LastSheetRow, it's probably unnecesarry
    }
}