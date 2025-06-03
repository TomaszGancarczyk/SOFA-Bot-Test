using Discord;
using Discord.WebSocket;


namespace SOFA_Bot_Test.Attendance
{
    internal class ButtonEventHandler
    {
        public static async Task Handler(SocketMessageComponent component)
        {
            EmbedBuilder updatedMessage;
            EmbedBuilder? message;
            ulong? currentMessageId;
            switch (component.Data.CustomId)
            {
                case "tournamentButton":
                    Logger.LogInformation($"Got tournament response to event question");
                    QuestionHandler.SetQuestionAnswear(component.Message.Id, "Tournament");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "baseCaptureButton":
                    Logger.LogInformation($"Got base capture response to event question");
                    QuestionHandler.SetQuestionAnswear(component.Message.Id, "Base Capture");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "dayOffButton":
                    Logger.LogInformation($"Got day off response to event question");
                    QuestionHandler.SetQuestionAnswear(component.Message.Id, "Day Off");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "brawlButton":
                    Logger.LogInformation($"Got brawl response to event question");
                    QuestionHandler.SetQuestionAnswear(component.Message.Id, "Brawl");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "presentButton":
                    message = null;
                    currentMessageId = BotHandler.GetCurrentMessageId();
                    if (currentMessageId == null)
                        RespondWithOldSignupError(component);
                    else if (component.Message.Id == currentMessageId)
                    {
                        Logger.LogInformation($"{component.User.GlobalName} clicked present on the signup");
                        message = await MemberHandler.SetMemberStatus(component.User, true);
                        if (message != null)
                        {
                            await component.RespondAsync(embed: message.Build(), ephemeral: true);
                            break;
                        }
                        updatedMessage = await SignupMessage.UpdateSignupMessage();
                        //message = await SignupMessage.CreateConfirmationMesasage("Present");
                        await component.UpdateAsync(attendanceMessage => attendanceMessage.Embed = updatedMessage.Build());
                        //await component.FollowupAsync(embed: message.Build(), ephemeral: true);
                        BotHandler.SetCurrentMessage(component.Message);
                    }
                    else
                        RespondWithOldSignupError(component);
                    break;
                case "absentButton":
                    message = null;
                    currentMessageId = BotHandler.GetCurrentMessageId();
                    if (currentMessageId == null)
                        RespondWithOldSignupError(component);
                    else if (component.Message.Id == currentMessageId)
                    {
                        Logger.LogInformation($"{component.User.GlobalName} clicked absent on the signup");
                        message = await MemberHandler.SetMemberStatus(component.User, false);
                        if (message != null)
                        {
                            await component.RespondAsync(embed: message.Build(), ephemeral: true);
                            break;
                        }
                        updatedMessage = await SignupMessage.UpdateSignupMessage();
                        //message = await SignupMessage.CreateConfirmationMesasage("Absent");
                        await component.UpdateAsync(attendanceMessage => attendanceMessage.Embed = updatedMessage.Build());
                        //await component.FollowupAsync(embed: message.Build(), ephemeral: true);
                        BotHandler.SetCurrentMessage(component.Message);
                    }
                    else
                        RespondWithOldSignupError(component);
                    break;
            }
        }
        private static async void RespondWithOldSignupError(SocketMessageComponent component)
        {
            Logger.LogWarning($"{component.User.GlobalName} interacted with old signup");
            EmbedBuilder message = await SignupMessage.CreateWrongSignupMesasage();
            await component.RespondAsync(embed: message.Build(), ephemeral: true);
        }
    }
}
