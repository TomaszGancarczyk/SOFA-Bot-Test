using Discord;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class CreateMessage
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");

        //TODO create messages
        internal async static Task<string> CreateTournamentMessage()
        {
            logger.LogInformation("{Time} - Creating tournament message", DateTime.Now);
            string messageContents = null;
            return messageContents;
        }
        internal async static Task<string> CreateBaseCaptureMessage()
        {
            logger.LogInformation("{Time} - Creating base capture message", DateTime.Now);
            string messageContents = null;
            return messageContents;
        }
        internal async static Task<string> CreateGoldenDropMessage()
        {
            logger.LogInformation("{Time} - Creating golden drop message", DateTime.Now);
            string messageContents = null;
            return messageContents;
        }
    }
}
