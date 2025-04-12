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
            IMessage channelMessage = null;
            EmbedBuilder embedMessage;
            bool doWePlayGoldenDrop;
            switch (eventDayOfWeek)
            {
                case DayOfWeek.Monday:
                    doWePlayGoldenDrop = await HandleGoldenDropQuestion(clanWarChannel);
                    if (doWePlayGoldenDrop)
                    {
                        embedMessage = await CreateMessage.CreateGoldenDropMessage();
                        channelMessage = await goldenDropChannel.SendMessageAsync("", false, embedMessage.Build());
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
                    channelMessage = await clanWarChannel.SendMessageAsync("", false, embedMessage.Build());
                    break;
                case DayOfWeek.Friday:
                    embedMessage = await CreateMessage.CreateTournamentMessage();
                    channelMessage = await clanWarChannel.SendMessageAsync("", false, embedMessage.Build());
                    break;
                case DayOfWeek.Saturday:
                    embedMessage = await CreateMessage.CreateTournamentMessage();
                    channelMessage = await clanWarChannel.SendMessageAsync("", false, embedMessage.Build());
                    break;
                case DayOfWeek.Sunday:
                    doWePlayGoldenDrop = await HandleGoldenDropQuestion(clanWarChannel);
                    if (doWePlayGoldenDrop)
                    {
                        embedMessage = await CreateMessage.CreateGoldenDropMessage();
                        channelMessage = await goldenDropChannel.SendMessageAsync("", false, embedMessage.Build());
                    }
                    else
                    {
                        embedMessage = await CreateMessage.CreateBaseCaptureMessage();
                        channelMessage = await clanWarChannel.SendMessageAsync("", false, embedMessage.Build());
                    }
                    break;
            }
            return channelMessage;
        }
        private async static Task<bool> HandleGoldenDropQuestion(IMessageChannel questionChannel)
        {
            bool questionAnswear = true;
            logger.LogInformation("{Time} - Sending question for base capture", DateTime.Now);
            //TODO post question and get return
            return await Task.FromResult(questionAnswear);
        }
    }
}
