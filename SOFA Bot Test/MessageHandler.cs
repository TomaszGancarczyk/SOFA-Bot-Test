using Discord;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class MessageHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");

        internal async static Task<IMessage> CreateMesage(IMessageChannel questionChannel, IMessageChannel clanWarChannel, IMessageChannel goldenDropChannel, DayOfWeek eventDayOfWeek)
        {
            logger.LogInformation("{Time} - Starting message creation", DateTime.Now);
            string eventType = await QuestionHandler.HandleEventQuestion(questionChannel);
            IMessage eventMessage = null;
            if (eventType == "Day Off")
            {
                logger.LogInformation("{Time} - We don't play tomorrow", DateTime.Now);
            }
            else
            {
                EmbedBuilder eventMessageBUilder = await CreateMessage.CreateAttendanceMessage(eventType);
            }
            //IMessage eventMessage = null;
            //EmbedBuilder embedMessage;
            //ComponentBuilder messageButton;
            //bool doWePlayGoldenDrop;
            //switch (eventDayOfWeek)
            //{
            //    case DayOfWeek.Monday:
            //        doWePlayGoldenDrop = await QuestionHandler.HandleGoldenDropQuestion(clanWarChannel);
            //        if (doWePlayGoldenDrop)
            //        {
            //            embedMessage = await CreateMessage.CreateGoldenDropMessage();
            //            messageButton = CreateButton.CreateAttendanceButton();
            //            eventMessage = await clanWarChannel.SendMessageAsync("", false, embedMessage.Build(), null, null, null, messageButton.Build());
            //            logger.LogInformation("{Time} - Sent golden drop message", DateTime.Now);
            //        }
            //        break;
            //    case DayOfWeek.Tuesday:
            //        logger.LogInformation("{Time} - We don't play tomorrow", DateTime.Now);
            //        break;
            //    case DayOfWeek.Wednesday:
            //        logger.LogInformation("{Time} - We don't play tomorrow", DateTime.Now);
            //        break;
            //    case DayOfWeek.Thursday:
            //        embedMessage = await CreateMessage.CreateTournamentMessage();
            //        messageButton = CreateButton.CreateAttendanceButton();
            //        eventMessage = await clanWarChannel.SendMessageAsync("", false, embedMessage.Build(), null, null, null, messageButton.Build());
            //        logger.LogInformation("{Time} - Sent tournament message", DateTime.Now);
            //        break;
            //    case DayOfWeek.Friday:
            //        embedMessage = await CreateMessage.CreateTournamentMessage();
            //        messageButton = CreateButton.CreateAttendanceButton();
            //        eventMessage = await clanWarChannel.SendMessageAsync("", false, embedMessage.Build(), null, null, null, messageButton.Build());
            //        logger.LogInformation("{Time} - Sent tournament message", DateTime.Now);
            //        break;
            //    case DayOfWeek.Saturday:
            //        embedMessage = await CreateMessage.CreateTournamentMessage();
            //        messageButton = CreateButton.CreateAttendanceButton();
            //        eventMessage = await clanWarChannel.SendMessageAsync("", false, embedMessage.Build(), null, null, null, messageButton.Build());
            //        logger.LogInformation("{Time} - Sent tournament message", DateTime.Now);
            //        break;
            //    case DayOfWeek.Sunday:
            //        doWePlayGoldenDrop = await QuestionHandler.HandleGoldenDropQuestion(clanWarChannel);
            //        if (doWePlayGoldenDrop)
            //        {
            //            embedMessage = await CreateMessage.CreateGoldenDropMessage();
            //            messageButton = CreateButton.CreateAttendanceButton();
            //            eventMessage = await clanWarChannel.SendMessageAsync("", false, embedMessage.Build(), null, null, null, messageButton.Build());
            //            logger.LogInformation("{Time} - Sent golden drop message", DateTime.Now);
            //        }
            //        else
            //        {
            //            embedMessage = await CreateMessage.CreateBaseCaptureMessage();
            //            messageButton = CreateButton.CreateAttendanceButton();
            //            eventMessage = await clanWarChannel.SendMessageAsync("", false, embedMessage.Build(), null, null, null, messageButton.Build());
            //            logger.LogInformation("{Time} - Sent base capture message", DateTime.Now);
            //        }
            //        break;
            //}

            return eventMessage;
        }
    }
}
