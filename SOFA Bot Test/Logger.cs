using Serilog;
namespace SOFA_Bot_Test
{
    internal class Logger
    {
        //private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Log");
        private static Serilog.Core.Logger log = new LoggerConfiguration()
            .WriteTo.File("log.txt", outputTemplate: "{dd:MM:yyyy HH:mm:ss} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
            .WriteTo.Console(outputTemplate: "{dd:MM:yyyy HH:mm:ss} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        internal static void LogCritical(string message)
        {
            log.Fatal("{DateTime.Now} - {message}", DateTime.Now.ToString("dd:MM:yyyy HH:mm:ss"), message);
        }
        internal static void LogError (string message)
        {
            log.Error("{DateTime.Now} - {message}", DateTime.Now.ToString("dd:MM:yyyy HH:mm:ss"), message);
        }
        internal static void LogWarning(string message)
        {
            log.Warning("{DateTime.Now} - {message}", DateTime.Now.ToString("dd:MM:yyyy HH:mm:ss"), message);
        }
        internal static void LogInformation(string message)
        {
            log.Information("{DateTime.Now} - {message}", DateTime.Now.ToString("dd:MM:yyyy HH:mm:ss"), message);
        }
    }
}