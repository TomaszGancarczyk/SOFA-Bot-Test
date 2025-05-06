using Discord;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test.Attendance
{
    internal class QuestionHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Attendance");
        private static bool WaitingForQuestionResponse;
        private static string QuestionResponse;
        private static IMessage QuestionMessage;

        internal static void SetQuestionAnswear(string questionResponse)
        {
            QuestionResponse = questionResponse;
            WaitingForQuestionResponse = false;
        }

        internal async static Task<string> HandleEventQuestion(IMessageChannel questionChannel)
        {
            QuestionMessage = null;
            WaitingForQuestionResponse = true;
            DayOfWeek eventDayOfWeek = Timer.GetEventDateTime().DayOfWeek;
            WaitingForQuestionResponse = true;
            logger.LogInformation("{Time} - Sending question for event", DateTime.Now);
            string questionMessageContent = "## What do we play tomorrow?";
            ComponentBuilder component = CreateButton.CreateQuestionButtons();
            QuestionMessage = await questionChannel.SendMessageAsync(questionMessageContent, components: component.Build());
            while (WaitingForQuestionResponse)
            {
                await Task.Delay(1000);
            }
            return QuestionResponse;
        }
        internal async static Task DeleteReminderMessage()
        {
            if (QuestionMessage != null)
            {
                logger.LogInformation("{Time} - Deleting question message", DateTime.Now);
                await QuestionMessage.DeleteAsync();
                QuestionMessage = null;
            }
        }
    }
}
