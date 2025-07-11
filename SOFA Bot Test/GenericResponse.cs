﻿using Discord;
using Discord.WebSocket;


namespace FOFA_Bot
{
    internal class GenericResponse
    {
        internal class Error
        {
            internal static async Task<EmbedBuilder> CantFindPlayer(SocketSlashCommand command)
            {
                EmbedBuilder embed = new();
                embed.WithColor(Color.Red);
                embed.WithTitle($"Couldn't find player {command.Data.Options.First().Value}");
                return embed;
            }
            internal static async Task<EmbedBuilder> NoPermission()
            {
                EmbedBuilder embed = new();
                embed.WithColor(Color.Red);
                embed.WithTitle($"You don't have permission to use this command");
                return embed;
            }
            internal static async Task<EmbedBuilder> Null()
            {
                EmbedBuilder embed = new();
                embed.WithColor(Color.Red);
                embed.WithTitle($"The field was empty");
                return embed;
            }
            internal static async Task<EmbedBuilder> NoSignupPermission()
            {
                EmbedBuilder embed = new();
                embed.WithColor(Color.Red);
                embed.WithTitle($"You don't have permission to use this signup");
                return embed;
            }
        }
        internal class Success
        {
            internal static async Task<EmbedBuilder> NewSignup()
            {
                EmbedBuilder embed = new();
                embed.WithColor(Color.Green);
                embed.WithTitle($"Creating new signup");
                return embed;
            }
            internal static async Task<EmbedBuilder> RemindersChanged(bool status)
            {
                EmbedBuilder embed = new();
                embed.WithColor(Color.Green);
                embed.WithTitle($"Reminder messages are changed to {status}");
                return embed;
            }
        }
    }
}
