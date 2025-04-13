using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class ButtonHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");

        public static async Task Handler(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case "yesButton":
                    logger.LogInformation("{Time} - Got yes reponse to golden drop question", DateTime.Now);
                    MessageHandler.SetQuestionAnswear(true);
                    break;
                case "noButton":
                    logger.LogInformation("{Time} - Got no reponse to golden drop question", DateTime.Now);
                    MessageHandler.SetQuestionAnswear(false);
                    break;
            }
        }
    }
}
