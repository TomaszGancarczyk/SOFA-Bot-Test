using Discord;
using Discord.WebSocket;
using FOFA_Bot.Attendance;
using FOFA_Bot.PlayerStats;


namespace FOFA_Bot
{
    internal class SlashCommandHandler
    {
        public static async Task Handler(SocketSlashCommand command)
        {
            EmbedBuilder? embed;
            string[] privilegedRoles;
            SocketGuildUser user;
            bool hasPermission;
            switch (command.Data.Name)
            {
                case "stats":
                    await command.DeferAsync(ephemeral: true);
                    string? playerName = command.Data.Options.First().Value.ToString();
                    if (playerName == null)
                    {
                        embed = await GenericResponse.Error.Null();
                        await command.FollowupAsync(embed: embed.Build());
                        break;
                    }
                    Logger.LogInformation($"User {command.User.GlobalName} used stats for {playerName}");
                    Stats player = Stats.CreateTestPlayer();
                    //Stats player = await ApiHandler.GetPlayerStats(playerName);
                    if (player != null)
                    {
                        embed = await StatsHandler.CreateStatsMessage(player);
                        string factionImage = await StatsHandler.GetFactionImage(player.Faction);
                        embed.WithThumbnailUrl($"attachment://{player.Faction.ToLower()}.webp");
                        await command.FollowupAsync($"Listing stats for {player.Username}");
                        await command.Channel.SendFileAsync(factionImage, embed: embed.Build());
                    }
                    else
                    {
                        embed = await GenericResponse.Error.CantFindPlayer(command);
                        await command.FollowupAsync(embed: embed.Build());
                    }
                    break;
                case "reminder-message":
                    await command.DeferAsync(ephemeral: true);
                    Logger.LogInformation($"User {command.User.GlobalName} used reminder-message");
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
                    if (embed != null)
                        await command.FollowupAsync(embed: embed.Build());
                    else
                        Logger.LogError("Couldn't get embed to respond to /reminder-message");
                    break;
                case "create-signup":
                    await command.DeferAsync(ephemeral: true);
                    Logger.LogInformation($"User {command.User.GlobalName} used create-signups");
                    hasPermission = false;
                    privilegedRoles = BotInfo.GetPrivilegedRoleNames();
                    user = await BotHandler.GetGuildUserByName(command.User.GlobalName);
                    embed = null;
                    foreach (string roleName in privilegedRoles)
                    {
                        if (user.Roles.Any(role => role.Name == roleName))
                        {
                            //TODO
                            _ = QuestionHandler.DeleteQuestionMessage();
                            if (command.Data.Options.First().Value.ToString() == "1")
                            {
                                _ = BotHandler.StartAttendanceEvent(true);
                            }
                            else if (command.Data.Options.First().Value.ToString() == "0")
                            {
                                _ = BotHandler.StartAttendanceEvent(false);
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
                    if (embed != null)
                        await command.FollowupAsync(embed: embed.Build());
                    else
                        Logger.LogError("Couldn't get embed to respond to /reminder-message");
                    break;
            }
        }
    }
}
