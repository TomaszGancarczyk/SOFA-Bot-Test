﻿using Discord;


namespace FOFA_Bot.Attendance
{
    public class CreateButton
    {
        private static readonly List<DayOfWeek> TournamentDays =
        [
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday
        ];
        private static readonly List<DayOfWeek> BaseCaptureDays =
        [
            DayOfWeek.Sunday
        ];
        private static readonly List<DayOfWeek> BrawlDays =
        [
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
        ];
        internal static ComponentBuilder CreateQuestionButtons()
        {
            DayOfWeek eventDayOfWeek = Timer.GetEventDateTime().DayOfWeek;
            ComponentBuilder component = new();
            if (TournamentDays.Contains(eventDayOfWeek))
                component.WithButton("Tournament", "tournamentButton", emote: new Emoji("⚔️"));
            if (BaseCaptureDays.Contains(eventDayOfWeek))
                component.WithButton("Base Capture", "baseCaptureButton", emote: new Emoji("👑"));
            if (BrawlDays.Contains(eventDayOfWeek))
                component.WithButton("Brawl", "brawlButton", emote: new Emoji("💵"));
            component.WithButton("Day Off", "dayOffButton", emote: new Emoji("🏖️"));
            return component;
        }
        internal static async Task<ComponentBuilder> CreateAttendanceButton()
        {
            ComponentBuilder button = new ComponentBuilder()
                .WithButton("Present", "presentButton", ButtonStyle.Success)
                .WithButton("Absent", "absentButton", ButtonStyle.Danger);
            Logger.LogInformation($"Created buttons");
            return button;
        }
    }
}
