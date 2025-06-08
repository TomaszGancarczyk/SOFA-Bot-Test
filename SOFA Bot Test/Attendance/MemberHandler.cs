using Discord;
using Discord.WebSocket;


namespace FOFA_Bot.Attendance
{
    internal class MemberHandler
    {
        private static readonly SocketGuild Guild = BotHandler.GetGuild();
        private static readonly string SofaRoleName = BotInfo.GetSofaRoleName();
        private static Dictionary<SocketGuildUser, bool?>? SofaMembers;
        internal static SocketRole GetRole(string roleName)
        {
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
            SocketGuildUser[] sofaMembers = [.. Guild.Users.Where(user => user.Roles.Any(role => role.Name == SofaRoleName))];
            foreach (SocketGuildUser member in sofaMembers)
            {
                if (!member.IsBot)
                    SofaMembers.Add(member, null);
            }
        }
        internal static async Task<EmbedBuilder> SetMemberStatus(SocketUser member, bool status)
        {
            SocketGuildUser? guildUser = Guild.Users.FirstOrDefault(user => user.Id == member.Id);
            if (guildUser == null)
            {
                Logger.LogError($"Cannot find membert with the name {member.GlobalName}");
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
                Logger.LogError($"{member.GlobalName} has no permission to use signups");
                return message;
            }
            else if (SofaMembers.Keys.Where(user => user.Id == member.Id) != null)
            {
                SocketGuildUser? key = SofaMembers.Keys.FirstOrDefault(user => user.Id == member.Id);
                if (key != null)
                {
                    SofaMembers[key] = status;
                    Logger.LogInformation($"Setting status {status} for {member.GlobalName}");
                }
                else
                    Logger.LogError($"Cannot find sofa member in SofaMembers with the name {member.GlobalName}");
                return null;
            }
            else
            {
                SofaMembers.Add(guildUser, status);
                Logger.LogInformation($"Addin {member.GlobalName} to member list with status {status}");
                return null;
            }
        }
    }
}