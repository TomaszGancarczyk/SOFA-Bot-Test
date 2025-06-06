using Discord;


namespace FOFA_Bot.Attendance
{
    internal class QuestionHandler
    {
        private static bool WaitingForQuestionResponse;
        private static string? QuestionResponse;
        private static IMessage? CurrentQuestionMessage = null;

        internal static void SetQuestionAnswear(ulong questionMessageId, string questionResponse)
        {
            if (CurrentQuestionMessage != null)
                if (questionMessageId == CurrentQuestionMessage.Id)
                {
                    QuestionResponse = questionResponse;
                    WaitingForQuestionResponse = false;
                }
                else Logger.LogError($"Got response from question message that has different ID than CurrentQuestionMessage");
            else Logger.LogError($"CurrentQuestionMessage is null");
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
        internal async static Task DeleteQuestionMessage()
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
