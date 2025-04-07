using Discord;
using Microsoft.Extensions.Logging;
using System;

namespace SOFA_Bot_Test
{
    internal class MessageHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");

        //TODO continue
        public static void SkipMessage()
        {
            logger.LogInformation("{Time} - We don't play tomorrow", DateTime.Now);
            TimeSpan eventTimer = Timer.GetEventTimeSpan();
        }
        public static void MessageInitalizer(IMessageChannel channel, string eventType)
        {
            logger.LogInformation("{Time} - Initalizing message for event {eventType}", DateTime.Now, eventType);
            switch (eventType)
            {
                case "Tournament":
                    HandleTournamentMessage(channel);
                    break;
                case "Base Capture":
                    HandleBaseCaptureMessage(channel);
                    break;
                case "Golden drop":
                    HandleGoldenDropMessage(channel);
                    break;
            }
        }
        private static void HandleTournamentMessage(IMessageChannel channel)
        {
            TimeSpan eventTimer = Timer.GetEventTimeSpan();
            Console.WriteLine("tournament");
        }
        private static void HandleBaseCaptureMessage(IMessageChannel channel)
        {
            TimeSpan eventTimer = Timer.GetEventTimeSpan();
            Console.WriteLine("base cap");
        }
        private static void HandleGoldenDropMessage(IMessageChannel channel)
        {
            TimeSpan eventTimer = Timer.GetEventTimeSpan();
            Console.WriteLine("golden drop");
        }
    }
}
