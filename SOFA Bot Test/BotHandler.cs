using Discord;
using Discord.WebSocket;
using FOFA_Bot.Attendance;
using FOFA_Bot.Nades;
using System.ComponentModel.Design;


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

            _ = StartEvents();
        }
        private static async Task StartEvents()
        {
            string? restoreCurrentMessageId = Attendance.Restore.Handler.CheckCurrentMessageId();
            if (restoreCurrentMessageId != null)
            {
                ulong CurrentMessageId = ulong.Parse(restoreCurrentMessageId);
                await ContinueAttendanceEvent(CurrentMessageId);
                if (CurrentMessage != null)
                {
                    //TODO handle restored message
                }
                else
                {
                    await StartAttendanceEvent(false);
                }
            }
            else
                await StartAttendanceEvent(false);
            ulong? currentMessageId;
            while (true)
            {
                if (CurrentMessage == null)
                    currentMessageId = null;
                else
                    currentMessageId = CurrentMessage.Id;
                while (currentMessageId != null || isDayOffRunning == true)
                {
                    Task.Delay(60000).Wait();
                    currentMessageId = ulong.Parse(Attendance.Restore.Handler.CheckCurrentMessageId());
                }
                Logger.LogInformation("There is no active event, running cooldown for new event");
                Task.Delay(7200000).Wait();
                currentMessageId = CurrentMessage.Id;
                if (currentMessageId == null)
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
            await Attendance.Restore.Handler.DeleteCurrentMessageIdFile();
            ulong? localCurrentMessageId = null;
            Logger.LogInformation($"Setting and getting event date time");
            await Attendance.Timer.SetEventDateTime(isToday);
            DateTime eventDateTime = Attendance.Timer.GetEventDateTime();
            Logger.LogInformation($"Event date time set for {eventDateTime}");
            if (QuestionChannel != null && SignupsChannel != null)
            {
                IMessage? tempCurrentMessage = await AttendanceMessageHandler.ValidateAndCreateMesage(QuestionChannel, SignupsChannel, isToday);
                if (tempCurrentMessage != null)
                {
                    if (eventDateTime > DateTime.Now)
                        Logger.LogInformation($"Event date time set for {eventDateTime}");
                    CurrentMessage = tempCurrentMessage;
                    await Attendance.Restore.Handler.CreateCurrentMessageIdFile(CurrentMessage.Id);
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
                await Attendance.Restore.Handler.DeleteCurrentMessageIdFile();
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
        private static async Task ContinueAttendanceEvent(ulong eventId)
        {
            CurrentMessage = await SignupsChannel.GetMessageAsync(eventId);
            if (CurrentMessage == null)
            {
                await Attendance.Restore.Handler.DeleteCurrentMessageIdFile();
                Logger.LogError("Cannot find signups channel or message by ID");
                return;
            }
            Logger.LogInformation("Found message in signups channel");
            IEmbed embed = CurrentMessage.Embeds.FirstOrDefault();
            long embedRemainingTime = (long.Parse(embed.Description.Split(":D")[0].Substring(3)))*1000;
            DateTime embedDateTime = DateTimeOffset.FromUnixTimeMilliseconds(embedRemainingTime).AddHours(2).DateTime;
            if (embedDateTime <= DateTime.Now)
            {
                Logger.LogError("Event already elapsed");
                CurrentMessage = null;
                await Attendance.Restore.Handler.DeleteCurrentMessageIdFile();
                return;
            }
            List<string> embedMembers = new();
            MemberHandler.SetMembers();
            Dictionary<SocketGuildUser, bool?> sofaMembers = await MemberHandler.GetSofaMembers();
            //TODO continue
            foreach (EmbedField field in embed.Fields)
            {
                foreach (string member in field.Value.Split("\n"))
                    embedMembers.Add(member);
            }
            // Update squad data based on fields if the time is right
            Console.WriteLine("e");
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
        //   create and delete backup message id file
        //   continue message validation
        // add people for reminder exceptions
        // handle player stats from API call

        //TODO Known Bugs
        // /create-signup when waiting for question response new message may be created

        //TODO Verify
        // DayOff colldown works properly
        // remove LastSheetRow, it's probably unnecesarry
    }
}