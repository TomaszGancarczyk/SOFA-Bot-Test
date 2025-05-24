using Discord;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test.Attendance
{
    internal class MessageHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Attendance");

        internal async static Task<IMessage> CreateMesage(IMessageChannel questionChannel, IMessageChannel clanWarChannel, IMessageChannel goldenDropChannel)
        {
            logger.LogInformation("{Time} - Starting message creation", DateTime.Now);
            string eventType = await QuestionHandler.HandleEventQuestion(questionChannel);
            SignupMessage.SetEventType(eventType);
            IMessage eventMessage = null;
            if (eventType == "Day Off")
            {
                logger.LogInformation("{Time} - We don't play tomorrow", DateTime.Now);
            }
            else
            {
                MemberHandler.SetMembers();
                EmbedBuilder embedMessage = await SignupMessage.CreateSignupMessage();
                ComponentBuilder messageButton = await CreateButton.CreateAttendanceButton();
                if (eventType == "Golden Drop")
                {
                    eventMessage = await goldenDropChannel.SendMessageAsync("", false, embedMessage.Build(), null, null, null, messageButton.Build());
                }
                else
                {
                    eventMessage = await clanWarChannel.SendMessageAsync("", false, embedMessage.Build(), null, null, null, messageButton.Build());
                }
                logger.LogInformation("{Time} - Signup message is sent", DateTime.Now);
            }
            return eventMessage;
        }
    }
}
