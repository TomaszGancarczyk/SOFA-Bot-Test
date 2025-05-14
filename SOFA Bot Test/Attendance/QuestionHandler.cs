using Discord;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test.Attendance
{
    internal class QuestionHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Attendance");
        private static bool WaitingForQuestionResponse;
        private static string QuestionResponse;
        private static IMessage CurrentQuestionMessage = null;

        internal static void SetQuestionAnswear(string questionResponse)
        {
            QuestionResponse = questionResponse;
            WaitingForQuestionResponse = false;
        }

        internal async static Task<string> HandleEventQuestion(IMessageChannel questionChannel)
        {
            DateTime eventDateTime = Timer.GetEventDateTime();
            string eventDateTimeDay = "";
            if (eventDateTime.Day == DateTime.Now.Day) eventDateTimeDay = "today";
            if (eventDateTime.Day == DateTime.Now.AddDays(1).Day) eventDateTimeDay = "tomorrow";
            CurrentQuestionMessage = null;
            WaitingForQuestionResponse = true;
            logger.LogInformation("{Time} - Sending question for event", DateTime.Now);
            string questionMessageContent = $"## What do we play {eventDateTimeDay}?";
            ComponentBuilder component = CreateButton.CreateQuestionButtons();
            CurrentQuestionMessage = await questionChannel.SendMessageAsync(questionMessageContent, components: component.Build());
            while (WaitingForQuestionResponse)
            {
                await Task.Delay(1000);
            }
            return QuestionResponse;
        }
        internal async static Task DeleteReminderMessage()
        {
            if (CurrentQuestionMessage != null)
            {
                logger.LogInformation("{Time} - Deleting question message", DateTime.Now);
                await CurrentQuestionMessage.DeleteAsync();
                CurrentQuestionMessage = null;
            }
        }
    }
}
