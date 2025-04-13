using Discord;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class MessageHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");
        private static bool WaitingForQuestionResponse;
        private static bool QuestionAnswear;

        internal async static Task<IMessage> CreateMesage(IMessageChannel clanWarChannel, IMessageChannel goldenDropChannel, DayOfWeek eventDayOfWeek)
        {
            logger.LogInformation("{Time} - Starting message creation", DateTime.Now);
            IMessage eventMessage = null;
            EmbedBuilder embedMessage;
            ComponentBuilder messageButton;
            bool doWePlayGoldenDrop;
            switch (eventDayOfWeek)
            {
                case DayOfWeek.Monday:
                    doWePlayGoldenDrop = await HandleGoldenDropQuestion(clanWarChannel);
                    if (doWePlayGoldenDrop)
                    {
                        embedMessage = await CreateMessage.CreateGoldenDropMessage();
                        messageButton = CreateButton.CreateAttendanceButton();
                        eventMessage = await clanWarChannel.SendMessageAsync("", false, embedMessage.Build(), null, null, null, messageButton.Build());
                        logger.LogInformation("{Time} - Sent golden drop message", DateTime.Now);
                    }
                    break;
                case DayOfWeek.Tuesday:
                    logger.LogInformation("{Time} - We don't play tomorrow", DateTime.Now);
                    break;
                case DayOfWeek.Wednesday:
                    logger.LogInformation("{Time} - We don't play tomorrow", DateTime.Now);
                    break;
                case DayOfWeek.Thursday:
                    embedMessage = await CreateMessage.CreateTournamentMessage();
                    messageButton = CreateButton.CreateAttendanceButton();
                    eventMessage = await clanWarChannel.SendMessageAsync("", false, embedMessage.Build(), null, null, null, messageButton.Build());
                    logger.LogInformation("{Time} - Sent tournament message", DateTime.Now);
                    break;
                case DayOfWeek.Friday:
                    embedMessage = await CreateMessage.CreateTournamentMessage();
                    messageButton = CreateButton.CreateAttendanceButton();
                    eventMessage = await clanWarChannel.SendMessageAsync("", false, embedMessage.Build(), null, null, null, messageButton.Build());
                    logger.LogInformation("{Time} - Sent tournament message", DateTime.Now);
                    break;
                case DayOfWeek.Saturday:
                    embedMessage = await CreateMessage.CreateTournamentMessage();
                    messageButton = CreateButton.CreateAttendanceButton();
                    eventMessage = await clanWarChannel.SendMessageAsync("", false, embedMessage.Build(), null, null, null, messageButton.Build());
                    logger.LogInformation("{Time} - Sent tournament message", DateTime.Now);
                    break;
                case DayOfWeek.Sunday:
                    doWePlayGoldenDrop = await HandleGoldenDropQuestion(clanWarChannel);
                    if (doWePlayGoldenDrop)
                    {
                        embedMessage = await CreateMessage.CreateGoldenDropMessage();
                        messageButton = CreateButton.CreateAttendanceButton();
                        eventMessage = await clanWarChannel.SendMessageAsync("", false, embedMessage.Build(), null, null, null, messageButton.Build());
                        logger.LogInformation("{Time} - Sent golden drop message", DateTime.Now);
                    }
                    else
                    {
                        embedMessage = await CreateMessage.CreateBaseCaptureMessage();
                        messageButton = CreateButton.CreateAttendanceButton();
                        eventMessage = await clanWarChannel.SendMessageAsync("", false, embedMessage.Build(), null, null, null, messageButton.Build());
                        logger.LogInformation("{Time} - Sent base capture message", DateTime.Now);
                    }
                    break;
            }
            return eventMessage;
        }
        private async static Task<bool> HandleGoldenDropQuestion(IMessageChannel questionChannel)
        {
            WaitingForQuestionResponse = true;
            logger.LogInformation("{Time} - Sending question for base capture", DateTime.Now);
            string questionMessageContent = "## Do we play Golden Drop?";
            ButtonBuilder yesButton = new ButtonBuilder()
            {
                Label = "Yes",
                CustomId = "yesButton",
                Style = ButtonStyle.Success
            };
            ButtonBuilder noButton = new ButtonBuilder()
            {
                Label = "No",
                CustomId = "noButton",
                Style = ButtonStyle.Danger
            };
            ComponentBuilder component = new ComponentBuilder()
                .WithButton(yesButton)
                .WithButton(noButton);
            IMessage questionMessage = await questionChannel.SendMessageAsync(questionMessageContent, components: component.Build());
            while (WaitingForQuestionResponse)
            {
                Task.Delay(1000);
            }
            switch (QuestionAnswear)
            {
                case true:
                    return await Task.FromResult(QuestionAnswear);
                case false:
                    return await Task.FromResult(QuestionAnswear);
            }
        }
        internal static void SetQuestionAnswear(bool answear)
        {
            QuestionAnswear = answear;
            WaitingForQuestionResponse = false;
        }
        //TODO post question and get return
    }
}
