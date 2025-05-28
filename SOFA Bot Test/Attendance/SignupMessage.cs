using Discord;
using Discord.WebSocket;


namespace SOFA_Bot_Test.Attendance
{
    internal class SignupMessage
    {
        private static string EventType;
        private static string EventMessageTitle;
        internal async static Task<EmbedBuilder> CreateSignupMessage()
        {
            Logger.LogInformation($"Creating {EventType} message");
            return await UpdateSignupMessage();
        }
        internal async static Task<EmbedBuilder> UpdateSignupMessage()
        {
            EmbedBuilder embed = new() { };
            DateTime eventDateTime = Timer.GetEventDateTime();
            long eventUnix = ((DateTimeOffset)eventDateTime).ToUnixTimeSeconds();
            switch (EventType)
            {
                case "Tournament":
                    embed.WithColor(Color.DarkGreen);
                    break;
                case "Golden Drop":
                    embed.WithColor(Color.Gold);
                    break;
                case "Base Capture":
                    embed.WithColor(Color.LightOrange);
                    break;
                case "Brawl":
                    embed.WithColor(Color.Green);
                    break;
            }
            EventMessageTitle = $"{eventDateTime.DayOfWeek} {EventType}";
            embed
                .WithTitle(EventMessageTitle)
                .WithDescription($"<t:{eventUnix}:D><t:{eventUnix}:t> - <t:{eventUnix}:R>");
            Dictionary<SocketGuildUser, bool?> sofaMembers = MemberHandler.GetSofaMembers();
            Dictionary<SocketGuildUser, bool?> rofaMembers = MemberHandler.GetRofaMembers();
            Dictionary<SocketGuildUser, bool?> unassignedMembers = MemberHandler.GetUnassignedMembers();
            int[] totalPresentAbsentUnsigned = [0, 0, 0];
            AddPresentAbsentUnsigned(totalPresentAbsentUnsigned, sofaMembers);
            AddPresentAbsentUnsigned(totalPresentAbsentUnsigned, unassignedMembers);
            if (EventType == "Golden Drop")
            {
                AddPresentAbsentUnsigned(totalPresentAbsentUnsigned, rofaMembers);
                string sofaField = "";
                foreach (var member in sofaMembers)
                {
                    sofaField += AddMemberAndStatus(member.Value, member.Key.DisplayName);
                }
                if (sofaField.Length > 0)
                    embed.AddField("SOFA", $"{sofaField}", true);
                string rofaField = "";
                foreach (var member in rofaMembers)
                {
                    rofaField += AddMemberAndStatus(member.Value, member.Key.DisplayName);
                }
                if (rofaField.Length > 0)
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
                List<ulong> handledMembersId = [];
                for (int i = 1; i <= 6; i++)
                {
                    squadMembers = "";
                    role = BotHandler.GetRole($"Squad {i}");
                    foreach (var member in sofaMembers)
                        if (member.Key.Roles.Contains(role) && !handledMembersId.Contains(member.Key.Id))
                        {
                            squadMembers += AddMemberAndStatus(member.Value, member.Key.DisplayName);
                            handledMembersId.Add(member.Key.Id);
                        }
                    if (squadMembers.Length > 0)
                        embed.AddField($"Squad {i}", squadMembers, true);
                }
                role = BotHandler.GetRole($"SOFA Reserve");
                squadMembers = "";
                foreach (var member in sofaMembers)
                    if (member.Key.Roles.Contains(role) && !handledMembersId.Contains(member.Key.Id))
                        squadMembers += AddMemberAndStatus(member.Value, member.Key.DisplayName);
                if (squadMembers.Length > 0)
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
            string footerMessage = $"____________________________________________________________________________________________________\n{totalPresentAbsentUnsigned[0]} Present, {totalPresentAbsentUnsigned[1]} Absent, {totalPresentAbsentUnsigned[2]} Unsigned";
            embed.WithFooter(footerMessage);
            return embed;
        }
        private static int[] AddPresentAbsentUnsigned(int[] totalPresentAbsentUnsigned, Dictionary<SocketGuildUser, bool?> memberDict)
        {

            foreach (var member in memberDict)
            {
                if (!member.Key.IsBot)
                {
                    if (member.Value == true)
                        totalPresentAbsentUnsigned[0] = totalPresentAbsentUnsigned[0] + 1;
                    if (member.Value == false)
                        totalPresentAbsentUnsigned[1] = totalPresentAbsentUnsigned[1] + 1;
                    if (member.Value == null)
                        totalPresentAbsentUnsigned[2] = totalPresentAbsentUnsigned[2] + 1;
                }
            }
            return totalPresentAbsentUnsigned;
        }
        internal async static Task<EmbedBuilder> CloseSignupMessage()
        {
            EmbedBuilder embed = await UpdateSignupMessage();
            embed.WithTitle($"{EventMessageTitle} - Signups closed");
            return embed;
        }
        internal async static Task<EmbedBuilder> CreateConfirmationMesasage(string status)
        {
            EmbedBuilder embed = new();
            DayOfWeek eventDayOfWeek = Timer.GetEventDateTime().DayOfWeek;
            if (status == "Present")
            {
                embed
                    .WithColor(Color.Green)
                    .WithTitle($"Signed Up for {eventDayOfWeek} {EventMessageTitle}");
            }
            if (status == "Absent")
            {
                embed
                    .WithColor(Color.Red)
                    .WithTitle($"Signed Off for {eventDayOfWeek} {EventMessageTitle}");
            }
            return embed;
        }
        internal async static Task<EmbedBuilder> CreateWrongSignupMesasage()
        {
            EmbedBuilder embed = new();
            embed
                .WithColor(Color.DarkGrey)
                .WithTitle($"This is signup is closed");
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
