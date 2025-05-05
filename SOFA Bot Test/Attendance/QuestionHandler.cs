using Discord;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test.Attendance
{
    internal class QuestionHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Attendance");
        private static bool WaitingForQuestionResponse;
        private static string QuestionResponse;

        internal static void SetQuestionAnswear(string questionResponse)
        {
            QuestionResponse = questionResponse;
            WaitingForQuestionResponse = false;
        }

        internal async static Task<string> HandleEventQuestion(IMessageChannel questionChannel)
        {
            WaitingForQuestionResponse = true;
            DayOfWeek eventDayOfWeek = Timer.GetEventDateTime().DayOfWeek;
            WaitingForQuestionResponse = true;
            logger.LogInformation("{Time} - Sending question for event", DateTime.Now);
            string questionMessageContent = "## What do we play tomorrow?";
            ComponentBuilder component = CreateButton.CreateQuestionButtons();
            IMessage questionMessage = await questionChannel.SendMessageAsync(questionMessageContent, components: component.Build());
            while (WaitingForQuestionResponse)
            {
                await Task.Delay(1000);
            }
            await questionMessage.DeleteAsync();
            return QuestionResponse;
        }

    }
}
