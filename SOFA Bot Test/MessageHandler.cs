using Discord;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class MessageHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");

        internal async static Task<IMessage> CreateMesage(IMessageChannel clanWarChannel, IMessageChannel goldenDropChannel, DayOfWeek eventDayOfWeek)
        {
            logger.LogInformation("{Time} - Starting message creation", DateTime.Now);
            bool doWePlayGoldenDrop;
            string messageContent = null;
            switch (eventDayOfWeek)
            {
                case DayOfWeek.Monday:
                    doWePlayGoldenDrop = await HandleGoldenDropQuestion(clanWarChannel);
                    if (doWePlayGoldenDrop)
                        messageContent = await CreateMessage.CreateGoldenDropMessage();
                    break;
                case DayOfWeek.Tuesday:
                    logger.LogInformation("{Time} - We don't play tomorrow", DateTime.Now);
                    break;
                case DayOfWeek.Wednesday:
                    logger.LogInformation("{Time} - We don't play tomorrow", DateTime.Now);
                    break;
                case DayOfWeek.Thursday:
                    messageContent = await CreateMessage.CreateTournamentMessage();
                    break;
                case DayOfWeek.Friday:
                    messageContent = await CreateMessage.CreateTournamentMessage();
                    break;
                case DayOfWeek.Saturday:
                    messageContent = await CreateMessage.CreateTournamentMessage();
                    break;
                case DayOfWeek.Sunday:
                    doWePlayGoldenDrop = await HandleGoldenDropQuestion(clanWarChannel);
                    if (doWePlayGoldenDrop)
                    {
                        messageContent = await CreateMessage.CreateGoldenDropMessage();
                    }
                    else
                    {
                        messageContent = await CreateMessage.CreateBaseCaptureMessage();
                    }
                    break;
            }
            //TODO Continue after message is created
            Console.WriteLine(messageContent);
            Console.ReadLine();
            return null;
        }
        private async static Task<bool> HandleGoldenDropQuestion(IMessageChannel questionChannel)
        {
            bool questionAnswear = false;
            logger.LogInformation("{Time} - Sending question for base capture", DateTime.Now);
            //TODO post question and get return
            return await Task.FromResult(questionAnswear);
        }
    }
}
