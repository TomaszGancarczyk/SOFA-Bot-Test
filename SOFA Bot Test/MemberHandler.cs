using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class MemberHandler
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");
        private static SocketGuild Guild = BotHandler.GetGuild();
        private static string SofaRoleName = BotInfo.GetSofaRoleName();
        private static Dictionary<SocketGuildUser, bool?> SofaMembers;
        private static string RofaRoleName = BotInfo.GetRofaRoleName();
        private static Dictionary<SocketGuildUser, bool?> RofaMembers;
        private static Dictionary<SocketGuildUser, bool?> UnassignedMembers;
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
            SocketRole sofaRole = Guild.Roles.FirstOrDefault(role => role.Name == SofaRoleName);
            SocketGuildUser[] sofaMembers = Guild.Users.Where(user => user.Roles.Contains(sofaRole)).ToArray();
            foreach (SocketGuildUser member in sofaMembers)
            {
                SofaMembers.Add(member, null);
            }
            RofaMembers = [];
            SocketRole rofaRole = Guild.Roles.FirstOrDefault(role => role.Name == RofaRoleName);
            SocketGuildUser[] rofaMembers = Guild.Users.Where(user => user.Roles.Contains(rofaRole)).ToArray();
            foreach (SocketGuildUser member in rofaMembers)
            {
                RofaMembers.Add(member, null);
            }
            UnassignedMembers = [];
        }
        internal static Dictionary<SocketGuildUser, bool?> GetRofaMembers()
        {
            return RofaMembers;
        }
        internal static Dictionary<SocketGuildUser, bool?> GetUnassignedMembers()
        {
            return UnassignedMembers;
        }
        internal static void SetMemberStatus(ulong memberId, bool status)
        {
            SocketGuildUser member = SofaMembers.Keys.FirstOrDefault(user => user.Id == memberId);
            if (member == null)
                member = RofaMembers.Keys.FirstOrDefault(user => user.Id == memberId);

            if (member == null)
            {
                member = Guild.Users.FirstOrDefault(user => user.Id == memberId);
                foreach (var role in member.Roles)
                {
                    if (role.Name == SofaRoleName)
                        SofaMembers.Add(member, status);
                    else if (role.Name == RofaRoleName)
                        RofaMembers.Add(member, status);
                    else
                        UnassignedMembers.Add(member, status);
                }
            }

            logger.LogInformation("{Time} - Setting status {status} for {member}", DateTime.Now, status, member.DisplayName);
        }
    }
}
