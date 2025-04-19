using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SOFA_Bot_Test
{
    internal class CreateMessage
    {
        private static readonly ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");
        private static string EventType;
        private static string EventMessageTitle;
        internal async static Task<EmbedBuilder> CreateAttendanceMessage()
        {
            logger.LogInformation("{Time} - Creating {eventType} message", DateTime.Now, EventType);
            return await UpdateAttendanceMessage();
        }
        internal async static Task<EmbedBuilder> UpdateAttendanceMessage()
        {
            EmbedBuilder embed = new() { };
            DateTime eventDateTime = Timer.GetEventDateTime();
            long eventUnix = ((DateTimeOffset)eventDateTime).ToUnixTimeSeconds();
            switch (EventType)
            {
                case "Tournament":
                    embed.WithColor(Color.Green);
                    break;
                case "Golden Drop":
                    embed.WithColor(Color.Gold);
                    break;
                case "Base Capture":
                    embed.WithColor(Color.DarkGreen);
                    break;
            }
            EventMessageTitle = $"{eventDateTime.DayOfWeek} {EventType}";
            embed
                .WithTitle(EventMessageTitle)
                .WithDescription($"<t:{eventUnix}:D><t:{eventUnix}:t> - <t:{eventUnix}:R>");
            Dictionary<SocketGuildUser, bool?> sofaMembers = MemberHandler.GetSofaMembers();
            Dictionary<SocketGuildUser, bool?> rofaMembers = MemberHandler.GetRofaMembers();
            Dictionary<SocketGuildUser, bool?> unassignedMembers = MemberHandler.GetUnassignedMembers();
            if (EventType == "Golden Drop")
            {
                string sofaField = "";
                foreach (var member in sofaMembers)
                {
                    sofaField += AddMemberAndStatus(member.Value, member.Key.DisplayName);
                }
                embed.AddField("SOFA", $"{sofaField}", true);
                string rofaField = "";
                foreach (var member in rofaMembers)
                {
                    rofaField += AddMemberAndStatus(member.Value, member.Key.DisplayName);
                }
                embed.AddField("ROFA", $"{rofaField}", true);
                if (unassignedMembers.Count > 0)
                {
                    string unassignedField = "";
                    foreach (var member in unassignedMembers)
                    {
                        unassignedField += AddMemberAndStatus(member.Value, member.Key.DisplayName);
                    }
                    embed.AddField("Unassigned", $"{unassignedField}", true);
                }
            }
            else
            {
                SocketRole role;
                string squadMembers;
                for (int i = 1; i <= 7; i++)
                {
                    squadMembers = "";
                    role = BotHandler.GetRole($"Squad {i}");
                    foreach (var member in sofaMembers)
                        if (member.Key.Roles.Contains(role))
                            squadMembers += AddMemberAndStatus(member.Value, member.Key.DisplayName);
                    if (squadMembers.Count() > 0)
                        embed.AddField($"Squad {i}", squadMembers, true);
                }
                role = BotHandler.GetRole($"SOFA Reserve");
                squadMembers = "";
                foreach (var member in sofaMembers)
                    if (member.Key.Roles.Contains(role))
                        squadMembers += AddMemberAndStatus(member.Value, member.Key.DisplayName);
                if (squadMembers.Count() > 0)
                    embed.AddField($"Reserve", squadMembers, true);
                if (unassignedMembers.Count > 0)
                {
                    string unassignedField = "";
                    foreach (var member in rofaMembers)
                    {
                        unassignedField += AddMemberAndStatus(member.Value, member.Key.DisplayName);
                    }
                    embed.AddField("Unassigned", $"{unassignedField}", true);
                }
            }
            return embed;
        }
        internal async static Task<EmbedBuilder> CreateConfirmationMesasage(string status)
        {
            EmbedBuilder embed = new();
            if (status == "Present")
            {
                embed.WithColor(Color.Green);
                embed.WithTitle($"Signed Up for {EventMessageTitle}");
            }
            if (status == "Absent")
            {
                embed.WithColor(Color.Red);
                embed.WithTitle($"Signed Off for {EventMessageTitle}");
            }
            return embed;
        }
        internal async static Task<EmbedBuilder> CreateWrongSignupMesasage()
        {
            EmbedBuilder embed = new();
            embed.WithColor(Color.DarkGrey);
            embed.WithTitle($"This is an old signup");
            return embed;
        }
        private static string AddMemberAndStatus(bool? status, string displayName)
        {
            if (status == null) return $"{new Emoji("⚫")} {displayName}\n";
            else if (status == true) return $"{new Emoji("🟢")} {displayName}\n";
            else if (status == false) return $"{new Emoji("🔴")} {displayName}\n";
            return null;
        }
        internal static void SetEventType(string eventType)
        {
            EventType = eventType;
        }
        internal static string GetEventType()
        {
            return EventType;
        }
    }
}
