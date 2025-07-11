﻿using Discord;
using Discord.WebSocket;


namespace FOFA_Bot.Attendance
{
    internal class SignupMessage
    {
        private static string? EventType;
        private static string? EventMessageTitle;
        internal static async Task<EmbedBuilder> CreateSignupMessage()
        {
            Logger.LogInformation($"Creating {EventType} message");
            return await UpdateSignupMessage();
        }
        internal static async Task<EmbedBuilder> UpdateSignupMessage()
        {
            EmbedBuilder embed = new();
            DateTime eventDateTime = Timer.GetEventDateTime();
            long eventUnix = ((DateTimeOffset)eventDateTime).ToUnixTimeSeconds();
            switch (EventType)
            {
                case "Tournament":
                    embed.WithColor(Color.DarkGreen);
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
                .WithDescription($"<t:{eventUnix}:D><t:{eventUnix}:t> - <t:{eventUnix}:R>\n" +
                $"Lineup: https://discord.com/channels/710884253457711134/1272270948115939339");
            Dictionary<SocketGuildUser, bool?> sofaMembers = await MemberHandler.GetSofaMembers();
            int[] totalPresentAbsentUnsigned = [0, 0, 0];
            AddPresentAbsentUnsigned(totalPresentAbsentUnsigned, sofaMembers);
            SocketRole? role;
            string? squadMembers;
            List<ulong> handledMembersId = [];
            List<IEmote> squadEmotes = [];
            squadEmotes.Add(new Emoji("🟦"));
            squadEmotes.Add(new Emoji("🟥"));
            squadEmotes.Add(new Emoji("🟫"));
            squadEmotes.Add(new Emoji("🟩"));
            squadEmotes.Add(new Emoji("🟨"));
            squadEmotes.Add(new Emoji("🟪"));
            for (int i = 1; i <= 6; i++)
            {
                squadMembers = "";
                role = BotHandler.GetRole($"Squad {i}");
                foreach (var member in sofaMembers)
                    if (member.Key.Roles.Contains(role) && !handledMembersId.Contains(member.Key.Id))
                    {
                        squadMembers += AddMemberAndStatus(member.Value, member.Key.Username);
                        handledMembersId.Add(member.Key.Id);
                    }
                if (squadMembers.Length > 0)
                {
                    embed.AddField($"{squadEmotes[i - 1]} Squad {i}", squadMembers, true);
                }
            }

            role = BotHandler.GetRole($"SOFA Reserve");
            squadMembers = "";
            foreach (var member in sofaMembers)
                if (member.Key.Roles.Contains(role) && !handledMembersId.Contains(member.Key.Id))
                {
                    squadMembers += AddMemberAndStatus(member.Value, member.Key.Username);
                    handledMembersId.Add(member.Key.Id);
                }
            if (squadMembers.Length > 0)
                embed.AddField($"{new Emoji("⬜")} Reserve", squadMembers, true);

            squadMembers = "";
            foreach (var member in sofaMembers)
                if (!handledMembersId.Contains(member.Key.Id))
                    squadMembers += AddMemberAndStatus(member.Value, member.Key.Username);
            if (squadMembers.Length > 0)
                embed.AddField($"{new Emoji("⬜")} Unassigned", squadMembers, true);

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
                        totalPresentAbsentUnsigned[0] += 1;
                    if (member.Value == false)
                        totalPresentAbsentUnsigned[1] += 1;
                    if (member.Value == null)
                        totalPresentAbsentUnsigned[2] += 1;
                }
            }
            return totalPresentAbsentUnsigned;
        }
        internal static async Task<EmbedBuilder> GetClosedSignupMessage()
        {
            EmbedBuilder embed = await UpdateSignupMessage();
            embed.WithTitle($"{EventMessageTitle} - Signups closed");
            return embed;
        }
        internal static async Task<EmbedBuilder> CreateConfirmationMesasage(string status)
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
        internal static async Task<EmbedBuilder> CreateWrongSignupMesasage()
        {
            EmbedBuilder embed = new();
            embed
                .WithColor(Color.DarkGrey)
                .WithTitle($"This is signup is closed");
            return embed;
        }
        internal static async Task<EmbedBuilder> CreateNoPermissionMessage()
        {
            EmbedBuilder embed = new();
            embed
                .WithColor(Color.DarkGrey)
                .WithTitle($"No permission to interact with this signup");
            return embed;
        }
        private static string? AddMemberAndStatus(bool? status, string Username)
        {
            if (status == null) return $"{new Emoji("⚫")} {Username}\n";
            else if (status == true) return $"{new Emoji("🟢")} {Username}\n";
            else if (status == false) return $"{new Emoji("🔴")} {Username}\n";
            return null;
        }
        internal static void SetEventType(string eventType)
        {
            EventType = eventType;
        }
        internal static string? GetEventType()
        {
            return EventType;
        }
    }
}
