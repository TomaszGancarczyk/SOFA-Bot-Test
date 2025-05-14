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
            string[] privilegedRoles;
            SocketGuildUser user;
            bool hasPermission;
            switch (command.Data.Name)
            {
                case "stats":
                    await command.DeferAsync();
                    string playerName = command.Data.Options.First().Value.ToString();
                    if (playerName == null)
                    {
                        embed = await GenericResponse.Error.Null();
                        await command.FollowupAsync(embed: embed.Build());
                        break;
                    }
                    logger.LogInformation("{Time} - User {user} asked for stats for {player}", DateTime.Now, command.User.GlobalName, playerName);
                    Stats player = await ApiHandler.GetPlayerStats(playerName);
                    if (player != null)
                    {
                        embed = await StatsHandler.CreateStatsMessage(command, player);
                    }
                    else
                    {
                        embed = await GenericResponse.Error.CantFindPlayer(command);
                    }
                    await command.FollowupAsync(embed: embed.Build());
                    break;
                case "reminder-message":
                    await command.DeferAsync(ephemeral: true);
                    hasPermission = false;
                    privilegedRoles = BotInfo.GetPrivilegedRoleNames();
                    user = await BotHandler.GetGuildUserByName(command.User.GlobalName);
                    embed = null;
                    foreach (string roleName in privilegedRoles)
                    {
                        if (user.Roles.Any(role => role.Name == roleName))
                        {
                            bool status = false;
                            if (command.Data.Options.First().Value.ToString() == "1")
                            {
                                status = true;
                                embed = await Reminder.CreateSignupCommandResponse(status);
                            }
                            else if (command.Data.Options.First().Value.ToString() == "0")
                            {
                                status = false;
                                embed = await Reminder.CreateSignupCommandResponse(status);
                            }
                            await Reminder.SetReminderPermission(status);
                            hasPermission = true;
                            embed = await GenericResponse.Success.RemindersChanged(status);
                            break;
                        }
                    }
                    if (!hasPermission)
                    {
                        embed = await GenericResponse.Error.NoPermission();
                    }
                    await command.FollowupAsync(embed: embed.Build());
                    break;
                case "create-signup":
                    await command.DeferAsync(ephemeral: true);
                    hasPermission = false;
                    privilegedRoles = BotInfo.GetPrivilegedRoleNames();
                    user = await BotHandler.GetGuildUserByName(command.User.GlobalName);
                    embed = null;
                    foreach (string roleName in privilegedRoles)
                    {
                        if (user.Roles.Any(role => role.Name == roleName))
                        {
                            QuestionHandler.DeleteReminderMessage();
                            if (command.Data.Options.First().Value.ToString() == "1")
                            {
                                BotHandler.StartAttendanceEvent(true);
                            }
                            else if (command.Data.Options.First().Value.ToString() == "0")
                            {
                                BotHandler.StartAttendanceEvent(false);
                            }
                            hasPermission = true;
                            embed = await GenericResponse.Success.NewSignup();
                            break;
                        }
                    }
                    if (!hasPermission)
                    {
                        embed = await GenericResponse.Error.NoPermission();
                    }
                    await command.FollowupAsync(embed: embed.Build());
                    break;
            }
        }
    }
}
