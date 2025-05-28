using Discord;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test.Attendance
{
    internal class QuestionHandler
    {
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
            Logger.LogInformation($"Sending question for event");
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
                Logger.LogInformation($"Deleting question message");
                await CurrentQuestionMessage.DeleteAsync();
                CurrentQuestionMessage = null;
            }
        }
    }
}
