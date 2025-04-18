﻿using Discord;
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

        internal static void InitializeBotHandler(DiscordSocketClient discord)
        {
            Discord = discord;
            Discord.ButtonExecuted += ButtonEventHandler.Handler;
            Guild = discord.GetGuild(BotInfo.GetGuildId());
            if (Guild == null)
            {
                logger.LogError("{Time} - Guild not found", DateTime.Now);
                throw new ArgumentNullException("Guild not found");
            }
            logger.LogInformation("{Time} - Found Guild: {Guild.Name}", DateTime.Now, Guild.Name);

            QuestionChannel = (IMessageChannel)Guild.GetChannel(BotInfo.GetQuestionChannelId());
            if (QuestionChannel == null)
            {
                logger.LogError("{Time} - Quesion channel not found", DateTime.Now);
                throw new ArgumentNullException("Question channel not found");
            }
            logger.LogInformation("{Time} - Found Question Channel: {Channel.Name}", DateTime.Now, QuestionChannel.Name);

            SignupsChannel = (IMessageChannel)Guild.GetChannel(BotInfo.GetSignupsChannelId());
            if (SignupsChannel == null)
            {
                logger.LogError("{Time} - Signups channel not found", DateTime.Now);
                throw new ArgumentNullException("Signups channel not found");
            }
            logger.LogInformation("{Time} - Found Signups Channel: {Channel.Name}", DateTime.Now, SignupsChannel.Name);

            GoldenDropChannel = (IMessageChannel)Guild.GetChannel(BotInfo.GetGoldenDropChannelId());
            if (GoldenDropChannel == null)
            {
                logger.LogError("{Time} - Golden Drop channel not found", DateTime.Now);
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
        private async static void StartEvent()
        {
            logger.LogInformation("{Time} - Starting event", DateTime.Now);
            logger.LogInformation("{Time} - Getting event date time", DateTime.Now);
            Timer.SetEventDateTimeForNextDay();
            var eventDateTime = Timer.GetEventDateTime();
            IMessage eventMessage = await MessageHandler.CreateMesage(QuestionChannel, SignupsChannel, GoldenDropChannel);
            TimeSpan reminderTimeSpan = eventDateTime - DateTime.Now.AddHours(1);
            if (reminderTimeSpan > TimeSpan.Zero) Reminder.Handle(reminderTimeSpan);
            //TODO Continue after reminder is handled
        }
    }
}