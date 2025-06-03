using Serilog;
namespace SOFA_Bot_Test
{
    internal class Logger
    {
        //private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Log");
        private static readonly Serilog.Core.Logger log = new LoggerConfiguration()
            .WriteTo.File("log.txt", outputTemplate:
        "[{Timestamp:dd:MM:yyyy HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("log_warnings.txt", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning, outputTemplate:
        "[{Timestamp:dd:MM:yyyy HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Console(outputTemplate:
        "[{Timestamp:dd:MM:yyyy HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        internal static void LogCritical(string message)
        {
            log.Fatal("{message}", message);
        }
        internal static void LogError(string message)
        {
            log.Error("{message}", message);
        }
        internal static void LogWarning(string message)
        {
            log.Warning("{message}", message);
        }
        internal static void LogInformation(string message)
        {
            log.Information("{message}", message);
        }
    }
}