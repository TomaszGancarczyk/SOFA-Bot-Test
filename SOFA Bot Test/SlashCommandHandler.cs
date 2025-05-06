using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SOFA_Bot_Test.Attendance;
using SOFA_Bot_Test.PlayerStats;

namespace SOFA_Bot_Test
{
    internal class SlashCommandHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");
        public static async Task Handler(SocketSlashCommand command)
        {
            EmbedBuilder embed;
            bool isEphemeral;
            List<SocketRole> privilegedRoles;
            SocketGuildUser user;
            bool hasPermission = false;
            switch (command.Data.Name)
            {
                case "stats":
                    await command.DeferAsync();
                    embed = null;
                    string playerName = command.Data.Options.First().Value.ToString();
                    logger.LogInformation("{Time} - User {user} asked for stats for {player}", DateTime.Now, command.User.GlobalName, playerName);
                    PlayerStatsDeserialized player = await ApiHandler.GetPlayerStats(playerName);
                    if (player != null)
                    {
                        embed = await StatsHandler.CreateStatsMessage(command, player);
                        isEphemeral = false;
                    }
                    else
                    {
                        embed = await StatsHandler.CreateStatsErrorMessage(command);
                        isEphemeral = true;
                    }
                    await command.FollowupAsync(embed: embed.Build(), ephemeral: isEphemeral);
                    break;
                case "reminderMessage":
                    await command.DeferAsync();
                    hasPermission = false;
                    privilegedRoles = await BotHandler.GetPrivilegedRoles();
                    user = await BotHandler.GetGuildUserByName(command.User.GlobalName);
                    foreach (SocketRole role in privilegedRoles)
                    {
                        if (user.Roles.Contains(role))
                        {
                            embed = null;
                            bool status = false;
                            if (command.Data.Options.First().Value.ToString() == "1")
                            {
                                status = true;
                                embed = await Reminder.CreateSignupCommandResponse(status);
                            }
                            if (command.Data.Options.First().Value.ToString() == "0")
                            {
                                status = false;
                                embed = await Reminder.CreateSignupCommandResponse(status);
                            }
                            logger.LogInformation("{Time} - Setting reminders to {status}", DateTime.Now, command.User.GlobalName, status);
                            await Reminder.SetReminderPermission(status);
                            hasPermission = true;
                            //respond success
                            break;
                        }
                        if (!hasPermission)
                        {
                            //respond no permission
                        }
                    }
                    break;
                case "createSignup":
                    await command.DeferAsync();
                    hasPermission = false;
                    privilegedRoles = await BotHandler.GetPrivilegedRoles();
                    user = await BotHandler.GetGuildUserByName(command.User.GlobalName);
                    foreach (SocketRole role in privilegedRoles)
                    {
                        if (user.Roles.Contains(role))
                        {
                            await QuestionHandler.DeleteReminderMessage();
                            await BotHandler.HandleEvent();
                            hasPermission = true;
                            //respond success
                            break;
                        }
                    }
                    if (!hasPermission)
                    {
                        //respond no permission
                    }
                    break;
            }
        }
    }
}
