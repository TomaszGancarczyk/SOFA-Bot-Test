using Discord;
using Discord.WebSocket;


namespace FOFA_Bot.Attendance
{
    internal class MemberHandler
    {
        private static readonly SocketGuild? Guild = BotHandler.GetGuild();
        private static readonly string SofaRoleName = BotInfo.GetSofaRoleName();
        private static Dictionary<SocketGuildUser, bool?>? SofaMembers;
        internal static SocketRole? GetRole(string roleName)
        {
            if (Guild == null) return null;
            SocketRole? role = Guild.Roles.FirstOrDefault(role => role.Name == roleName);
            if (role != null)
                return role;
            else
            {
                Logger.LogCritical($"Couldn't get role {roleName} in MembersHandler.GetRole");
                return null;
            }
        }
        internal static Task<Dictionary<SocketGuildUser, bool?>> GetSofaMembers()
        {
            if (SofaMembers != null)
                return Task.FromResult(SofaMembers);
            else
            {
                Logger.LogError("SofaMembers is null");
                return null;
            }
        }
        internal static void SetMembers()
        {
            SofaMembers = [];
            if (Guild == null)
            {
                Logger.LogError($"Connot set members due to Guild being null");
                return;
            }
            SocketGuildUser[] sofaMembers = [.. Guild.Users.Where(user => user.Roles.Any(role => role.Name == SofaRoleName))];
            foreach (SocketGuildUser member in sofaMembers)
            {
                if (!member.IsBot)
                    SofaMembers.Add(member, null);
            }
        }
        internal static async Task<EmbedBuilder?> SetMemberStatus(SocketUser member, bool status)
        {
            if (Guild == null)
                return null;
            SocketGuildUser? guildUser = Guild.Users.FirstOrDefault(user => user.Id == member.Id);
            if (guildUser == null)
            {
                Logger.LogError($"Cannot find membert with the name {member.Username}");
                return null;
            }
            else if (SofaMembers == null)
            {
                Logger.LogError($"SofaMembers is null");
                return null;
            }
            else if (guildUser.Roles.All(role => role.Name != SofaRoleName))
            {
                EmbedBuilder message = await GenericResponse.Error.NoSignupPermission();
                Logger.LogError($"{member.Username} has no permission to use signups");
                return message;
            }
            else if (SofaMembers.Keys.Where(user => user.Id == member.Id) != null)
            {
                SocketGuildUser? key = SofaMembers.Keys.FirstOrDefault(user => user.Id == member.Id);
                if (key == null)
                {
                    Logger.LogWarning($"Cannot find sofa member in SofaMembers with the name {member.Username}, adding new member with status {status}");
                    SofaMembers.Add(guildUser, status);
                }
                else
                {
                    SofaMembers[key] = status;
                    Logger.LogInformation($"Setting status {status} for {member.Username}");
                }
                if (status == false)
                {
                    EmbedBuilder embed = new();
                    embed.WithColor(Color.Red);
                    embed.WithTitle($"Don't forget to type in leave :3\n" +
                        $"https://discord.com/channels/710884253457711134/1170461909829566485");
                    return embed;
                }
                return null;
            }
            else
            {
                SofaMembers.Add(guildUser, status);
                Logger.LogInformation($"Adding {member.Username} to member list with status {status}");
                if (status == false)
                {
                    EmbedBuilder embed = new();
                    embed.WithColor(Color.Red);
                    embed.WithTitle($"Don't forget to type in leave :3\n" +
                        $"https://discord.com/channels/710884253457711134/1170461909829566485");
                    return embed;
                }
                return null;
            }
        }
    }
}