using Discord;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class MessageHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");

        internal async static Task<IMessage> CreateMesage(IMessageChannel questionChannel, IMessageChannel clanWarChannel, IMessageChannel goldenDropChannel)
        {
            logger.LogInformation("{Time} - Starting message creation", DateTime.Now);
            string eventType = await QuestionHandler.HandleEventQuestion(questionChannel);
            CreateMessage.SetEventType(eventType);
            IMessage eventMessage = null;
            if (eventType == "Day Off")
            {
                logger.LogInformation("{Time} - We don't play tomorrow", DateTime.Now);
            }
            else
            {
                MemberHandler.SetMembers();
                EmbedBuilder embedMessage = await CreateMessage.CreateAttendanceMessage();
                ComponentBuilder messageButton = CreateButton.CreateAttendanceButton();
                if (eventType == "Golden Drop")
                {
                    eventMessage = await goldenDropChannel.SendMessageAsync("", false, embedMessage.Build(), null, null, null, messageButton.Build());
                }
                else
                {
                    eventMessage = await clanWarChannel.SendMessageAsync("", false, embedMessage.Build(), null, null, null, messageButton.Build());
                }
            }
            return eventMessage;
        }
    }
}
