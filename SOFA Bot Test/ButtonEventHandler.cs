using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class ButtonEventHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");

        public static async Task Handler(SocketMessageComponent component)
        {
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
                case "presentButton":
                    logger.LogInformation("{Time} - {User} clicked present", DateTime.Now, component.User.GlobalName);
                    break;
                case "absentButton":
                    logger.LogInformation("{Time} - {User} clicked absent", DateTime.Now, component.User.GlobalName);
                    break;
            }
        }
    }
}
