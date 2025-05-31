using Discord;
using Discord.WebSocket;


namespace SOFA_Bot_Test.Attendance
{
    internal class MemberHandler
    {
        private static readonly SocketGuild Guild = BotHandler.GetGuild();
        private static readonly string SofaRoleName = BotInfo.GetSofaRoleName();
        private static Dictionary<SocketGuildUser, bool?> SofaMembers;
        internal static SocketRole GetRole(string roleName)
        {
            return Guild.Roles.FirstOrDefault(role => role.Name == roleName);
        }
        internal static Dictionary<SocketGuildUser, bool?> GetSofaMembers()
        {
            return SofaMembers;
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
            SocketGuildUser guildUser = Guild.Users.FirstOrDefault(user => user.Id == member.Id);
            if (guildUser.Roles.All(role => role.Name != SofaRoleName))
            {
                EmbedBuilder message = await GenericResponse.Error.NoSignupPermission(member);
                Logger.LogError($"{member.GlobalName} has no permission to use signups");
                return message;
            }
            else if (SofaMembers.Keys.Where(user => user.Id == member.Id) != null)
            {
                SocketGuildUser key = SofaMembers.Keys.FirstOrDefault(user => user.Id == member.Id);
                SofaMembers[key] = status;
                Logger.LogInformation($"Setting status {status} for {member.GlobalName}");
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
