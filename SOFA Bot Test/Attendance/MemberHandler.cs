using Discord.WebSocket;


namespace SOFA_Bot_Test.Attendance
{
    internal class MemberHandler
    {
        private static readonly SocketGuild Guild = BotHandler.GetGuild();
        private static readonly string SofaRoleName = BotInfo.GetSofaRoleName();
        private static Dictionary<SocketGuildUser, bool?> SofaMembers;
        private static readonly string RofaRoleName = BotInfo.GetRofaRoleName();
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
            SocketGuildUser[] sofaMembers = [.. Guild.Users.Where(user => user.Roles.Any(role => role.Name == SofaRoleName))];
            foreach (SocketGuildUser member in sofaMembers)
            {
                SofaMembers.Add(member, null);
            }
            RofaMembers = [];
            SocketGuildUser[] rofaMembers = [.. Guild.Users.Where(user => user.Roles.Any(role => role.Name == RofaRoleName))];
            foreach (SocketGuildUser member in rofaMembers)
            {
                if (!member.Roles.Any(role => role.Name == SofaRoleName))
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
        internal static void SetMemberStatus(SocketUser member, bool status)
        {
            if (SofaMembers.Keys.Where(user => user.Id == member.Id) != null)
            {
                SocketGuildUser key = SofaMembers.Keys.FirstOrDefault(user => user.Id == member.Id);
                SofaMembers[key] = status;
            }
            else if (RofaMembers.Keys.Where(user => user.Id == member.Id) != null)
            {
                SocketGuildUser key = RofaMembers.Keys.FirstOrDefault(user => user.Id == member.Id);
                RofaMembers[key] = status;
            }
            else if (UnassignedMembers.Keys.Where(user => user.Id == member.Id) != null)
            {
                SocketGuildUser key = UnassignedMembers.Keys.FirstOrDefault(user => user.Id == member.Id);
                UnassignedMembers[key] = status;
            }
            else
            {
                SocketGuildUser newMember = Guild.Users.FirstOrDefault(user => user.Id == member.Id);
                UnassignedMembers.Add(newMember, status);
            }

            Logger.LogInformation($"Setting status {status} for {member.GlobalName}");
        }
    }
}
