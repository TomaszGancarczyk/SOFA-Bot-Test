

namespace FOFA_Bot.Attendance.Restore
{
    internal class Handler
    {
        private const string FilePath = "CurrentMessageIdBackup.txt";
        internal static string? CheckCurrentMessageId()
        {
            if (File.Exists(FilePath))
            {
                Logger.LogInformation("Found saved attendance CurrentMessageIdBackup.txt");
                string? currentMessageId = File.ReadAllText(FilePath);
                return currentMessageId;
            }
            else
            {
                Logger.LogInformation("Cannot find attendance CurrentMessageIdBackup.txt");
                return null;
            }
        }
        internal static Task CreateCurrentMessageIdFile(ulong messageId)
        {
            Logger.LogInformation("Creating CurrentMessageIdBackup.txt");
            File.WriteAllText(FilePath, messageId.ToString());
            return Task.CompletedTask;
        }
        internal static Task DeleteCurrentMessageIdFile()
        {
            if (File.Exists(FilePath))
            {
                Logger.LogInformation("Deleting CurrentMessageIdBackup.txt");
                File.Delete(FilePath);
            }
            return Task.CompletedTask;
        }
    }
}