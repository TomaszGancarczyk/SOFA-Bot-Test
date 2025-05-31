using Discord;
using Discord.WebSocket;
using SOFA_Bot_Test.Attendance;


namespace SOFA_Bot_Test
{
    internal class GenericResponse
    {
        internal class Error
        {
            internal async static Task<EmbedBuilder> CantFindPlayer(SocketSlashCommand command)
            {
                EmbedBuilder embed = new();
                embed.WithColor(Color.Red);
                embed.WithTitle($"Couldn't find player {command.Data.Options.First().Value}");
                return embed;
            }
            internal async static Task<EmbedBuilder> NoPermission()
            {
                EmbedBuilder embed = new();
                embed.WithColor(Color.Red);
                embed.WithTitle($"You don't have permission to use this command");
                return embed;
            }
            internal async static Task<EmbedBuilder> Null()
            {
                EmbedBuilder embed = new();
                embed.WithColor(Color.Red);
                embed.WithTitle($"The field was empty");
                return embed;
            }
            internal static async Task<EmbedBuilder> NoSignupPermission(SocketUser user)
            {
                EmbedBuilder embed = new();
                embed.WithColor(Color.Red);
                embed.WithTitle($"You don't have permission to use this signup");
                return embed;
            }
        }
        internal class Success
        {
            internal async static Task<EmbedBuilder> NewSignup()
            {
                EmbedBuilder embed = new();
                embed.WithColor(Color.Green);
                embed.WithTitle($"Creating new signup");
                return embed;
            }
            internal async static Task<EmbedBuilder> RemindersChanged(bool status)
            {
                EmbedBuilder embed = new();
                embed.WithColor(Color.Green);
                embed.WithTitle($"Reminder messages are changed to {status}");
                return embed;
            }
        }
    }
}
