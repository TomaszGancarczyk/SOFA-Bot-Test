using Discord;


namespace SOFA_Bot_Test.Attendance
{
    internal class MessageHandler
    {
        internal async static Task<IMessage> CreateMesage(IMessageChannel questionChannel, IMessageChannel clanWarChannel, IMessageChannel goldenDropChannel)
        {
            Logger.LogInformation($"Starting message creation");
            string eventType = await QuestionHandler.HandleEventQuestion(questionChannel);
            SignupMessage.SetEventType(eventType);
            IMessage eventMessage = null;
            if (eventType == "Day Off")
            {
                Logger.LogInformation($"We don't play tomorrow");
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
                Logger.LogInformation($"Signup message is sent");
            }
            return eventMessage;
        }
    }
}
