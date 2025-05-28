using Discord;
using Discord.WebSocket;


namespace SOFA_Bot_Test.Attendance
{
    internal class ButtonEventHandler
    {
        public static async Task Handler(SocketMessageComponent component)
        {
            EmbedBuilder updatedMessage;
            EmbedBuilder confirmationMessage;
            ulong? currentMessageId;
            switch (component.Data.CustomId)
            {
                case "tournamentButton":
                    Logger.LogInformation($"Got tournament response to event question");
                    QuestionHandler.SetQuestionAnswear("Tournament");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "goldenDropButton":
                    Logger.LogInformation($"Got golden drop response to event question");
                    QuestionHandler.SetQuestionAnswear("Golden Drop");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "baseCaptureButton":
                    Logger.LogInformation($"Got base capture response to event question");
                    QuestionHandler.SetQuestionAnswear("Base Capture");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "dayOffButton":
                    Logger.LogInformation($"Got day off response to event question");
                    QuestionHandler.SetQuestionAnswear("Day Off");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "brawlButton":
                    Logger.LogInformation($"Got brawl response to event question");
                    QuestionHandler.SetQuestionAnswear("Brawl");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "presentButton":
                    currentMessageId = BotHandler.GetCurrentMessageId();
                    if (currentMessageId == null)
                    {
                        RespondWithOldSignupError(component);
                    }
                    else if (component.Message.Id == currentMessageId)
                    {
                        Logger.LogInformation($"{component.User.GlobalName} clicked present on the signup");
                        MemberHandler.SetMemberStatus(component.User, true);
                        updatedMessage = await SignupMessage.UpdateSignupMessage();
                        confirmationMessage = await SignupMessage.CreateConfirmationMesasage("Present");
                        await component.UpdateAsync(message => message.Embed = updatedMessage.Build());
                        await component.FollowupAsync(embed: confirmationMessage.Build(), ephemeral: true);
                        BotHandler.SetCurrentMessage(component.Message);
                    }
                    else
                    {
                        RespondWithOldSignupError(component);
                    }
                    break;
                case "absentButton":
                    currentMessageId = BotHandler.GetCurrentMessageId();
                    if (currentMessageId == null)
                    {
                        RespondWithOldSignupError(component);
                    }
                    else if (component.Message.Id == currentMessageId)
                    {
                        Logger.LogInformation($"{component.User.GlobalName} clicked absent on the signup");
                        MemberHandler.SetMemberStatus(component.User, false);
                        updatedMessage = await SignupMessage.UpdateSignupMessage();
                        confirmationMessage = await SignupMessage.CreateConfirmationMesasage("Absent");
                        await component.UpdateAsync(message => message.Embed = updatedMessage.Build());
                        await component.FollowupAsync(embed: confirmationMessage.Build(), ephemeral: true);
                        BotHandler.SetCurrentMessage(component.Message);
                    }
                    else
                    {
                        RespondWithOldSignupError(component);
                    }
                    break;
            }
        }
        private static async void RespondWithOldSignupError(SocketMessageComponent component)
        {
            Logger.LogInformation($"{component.User.GlobalName} interacted with old signup");
            EmbedBuilder message = await SignupMessage.CreateWrongSignupMesasage();
            await component.RespondAsync(embed: message.Build(), ephemeral: true);
        }
    }
}
