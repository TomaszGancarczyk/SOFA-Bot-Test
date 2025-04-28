using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test.Attendance
{
    internal class ButtonEventHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Attendance");

        public static async Task Handler(SocketMessageComponent component)
        {
            EmbedBuilder updatedMessage;
            EmbedBuilder confirmationMessage;
            ulong currentMessageId;
            switch (component.Data.CustomId)
            {
                case "tournamentButton":
                    logger.LogInformation("{Time} - Got tournament response to event question", DateTime.Now);
                    QuestionHandler.SetQuestionAnswear("Tournament");
                    break;
                case "goldenDropButton":
                    logger.LogInformation("{Time} - Got golden drop response to event question", DateTime.Now);
                    QuestionHandler.SetQuestionAnswear("Golden Drop");
                    break;
                case "baseCaptureButton":
                    logger.LogInformation("{Time} - Got base capture response to event question", DateTime.Now);
                    QuestionHandler.SetQuestionAnswear("Base Capture");
                    break;
                case "dayOffButton":
                    logger.LogInformation("{Time} - Got day off response to event question", DateTime.Now);
                    QuestionHandler.SetQuestionAnswear("Day Off");
                    break;
                case "brawlButton":
                    logger.LogInformation("{Time} - Got brawl response to event question", DateTime.Now);
                    QuestionHandler.SetQuestionAnswear("Brawl");
                    break;
                case "presentButton":
                    currentMessageId = BotHandler.GetCurrentMessageId();
                    if (component.Message.Id == currentMessageId)
                    {
                        logger.LogInformation("{Time} - {User} clicked present", DateTime.Now, component.User.GlobalName);
                        MemberHandler.SetMemberStatus(component.User, true);
                        updatedMessage = await CreateMessage.UpdateAttendanceMessage();
                        confirmationMessage = await CreateMessage.CreateConfirmationMesasage("Present");
                        await component.UpdateAsync(message => message.Embed = updatedMessage.Build());
                        await component.FollowupAsync(embed: confirmationMessage.Build(), ephemeral: true);
                        BotHandler.SetCurrentMessage(component.Message);
                    }
                    else
                    {
                        logger.LogInformation("{Time} - {User} interacted with old message", DateTime.Now, component.User.GlobalName);
                        confirmationMessage = await CreateMessage.CreateWrongSignupMesasage();
                        await component.RespondAsync(embed: confirmationMessage.Build(), ephemeral: true);
                    }
                    break;
                case "absentButton":
                    currentMessageId = BotHandler.GetCurrentMessageId();
                    if (component.Message.Id == currentMessageId)
                    {
                        logger.LogInformation("{Time} - {User} clicked absent", DateTime.Now, component.User.GlobalName);
                        MemberHandler.SetMemberStatus(component.User, false);
                        updatedMessage = await CreateMessage.UpdateAttendanceMessage();
                        confirmationMessage = await CreateMessage.CreateConfirmationMesasage("Absent");
                        await component.UpdateAsync(message => message.Embed = updatedMessage.Build());
                        await component.FollowupAsync(embed: confirmationMessage.Build(), ephemeral: true);
                        BotHandler.SetCurrentMessage(component.Message);
                    }
                    else
                    {
                        logger.LogInformation("{Time} - {User} interacted with old signup", DateTime.Now, component.User.GlobalName);
                        confirmationMessage = await CreateMessage.CreateWrongSignupMesasage();
                        await component.RespondAsync(embed: confirmationMessage.Build(), ephemeral: true);
                    }
                    break;
            }
        }
    }
}
