using Discord;

namespace FOFA_Bot.Nades
{
    internal class NadeHandler
    {
        private static ulong? CurrentMessageId;
        internal static async Task StartNadeEvent()
        {
            if (CurrentMessageId != null)
            {
                Logger.LogWarning("CurrentMessage for Nades is not null");
                return;
            }
            IMessageChannel nadeChannel = BotHandler.GetNadeChannel();
            if (nadeChannel == null)
            {
                Logger.LogWarning("NadeChannel is null");
                return;
            }
            var grenadeChoicesNames = await NadeMessageHandler.Handle(nadeChannel);
            if (CurrentMessageId == null)
            {
                Logger.LogWarning("CurrentMessage for Nades is null");
                return;
            }
            await NadeGoogleSheet.SaveNadePollResultsToSheet(grenadeChoicesNames);
            CurrentMessageId = null;
        }
        internal static void SetCurrentMessageId(ulong currentMessageId)
        {
            CurrentMessageId = currentMessageId;
        }
    }
}
