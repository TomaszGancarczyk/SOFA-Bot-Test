using Discord;


namespace FOFA_Bot.Attendance
{
    internal class MessageHandler
    {
        private static DateTime? ResponseTime = null;
        internal async static Task<IMessage?> ValidateAndCreateMesage(IMessageChannel questionChannel, IMessageChannel clanWarChannel)
        {
            Logger.LogInformation($"Starting message creation");
            string eventType = await QuestionHandler.HandleEventQuestion(questionChannel);
            SignupMessage.SetEventType(eventType);
            IMessage? response;
            if (ResponseTime == null)
            {
                ResponseTime = DateTime.Now;
                response = await CreateMesage(eventType, clanWarChannel);
                return response;
            }
            else
            {
                if (DateTime.Now - ResponseTime > TimeSpan.FromSeconds(5))
                {
                    ResponseTime = DateTime.Now;
                    response = await CreateMesage(eventType, clanWarChannel);
                    return response;
                }
                else
                {
                    Logger.LogWarning("Tried to create multiple signup messages");
                    return null;
                }
            }
        }
        private async static Task<IMessage?> CreateMesage(string eventType, IMessageChannel clanWarChannel)
        {
            if (eventType == "Day Off")
            {
                Logger.LogInformation($"We don't play tomorrow");
                return null;
            }
            else
            {
                MemberHandler.SetMembers();
                EmbedBuilder embedMessage = await SignupMessage.CreateSignupMessage();
                ComponentBuilder messageButton = await CreateButton.CreateAttendanceButton();
                IMessage? eventMessage = await clanWarChannel.SendMessageAsync("", false, embedMessage.Build(), null, null, null, messageButton.Build());
                Logger.LogInformation($"Signup message is sent");
                return eventMessage;
            }
        }
    }
}
