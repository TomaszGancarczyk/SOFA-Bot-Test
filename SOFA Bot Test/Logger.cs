using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFA_Bot_Test
{
    internal class Logger
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");
        internal static void LogCritical(string message)
        {
            logger.LogCritical("{DateTime.Now} - {message}", DateTime.Now, message);
        }
        internal static void LogWarning(string message)
        {
            logger.LogWarning("{DateTime.Now} - {message}", DateTime.Now, message);
        }
        internal static void LogInformation(string message)
        {
            logger.LogInformation("{DateTime.Now} - {message}", DateTime.Now, message);
        }

    }
}
