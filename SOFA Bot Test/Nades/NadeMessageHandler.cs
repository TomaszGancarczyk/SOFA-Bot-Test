using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOFA_Bot.Nades
{
    internal class NadeMessageHandler
    {
        internal static async Task<List<List<string>>> Handle(IMessageChannel nadeChannel)
        {
            PollProperties poll = await CreateNadesMessage();
            IUserMessage currentMessage = await nadeChannel.SendMessageAsync(poll: poll);
            NadeHandler.SetCurrentMessageId(currentMessage.Id);
            while (true)
            {
                if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday && DateTime.Now.Hour == 14)
                    break;
                Task.Delay(1800000).Wait();
            }
            Logger.LogInformation("Closing nade poll");
            await currentMessage.EndPollAsync(default);
            Task.Delay(60000).Wait();
            List<List<string>> grenadeChoicesNames = await ConvertPollChoicesToList(currentMessage);
            return grenadeChoicesNames;
        }
        private static async Task<PollProperties> CreateNadesMessage()
        {
            Logger.LogInformation("Creating nade poll");
            var poll = new PollProperties
            {
                Question = new()
                {
                    Text = "Which grenades do you need for this week? :3"
                },
                Duration = 72,
                Answers = [
                    new PollMediaProperties
                    {
                        Text = "Snowstorm",
                        Emoji = new Emoji("❄️")
                    },
                    new PollMediaProperties
                    {
                        Text = "Tar",
                        Emoji = new Emoji("⚫")
                    },
                    new PollMediaProperties
                    {
                        Text = "Frag",
                        Emoji = new Emoji("🧨")
                    },
                    new PollMediaProperties
                    {
                        Text = "Plantain",
                        Emoji = new Emoji("💚")
                    },
                    new PollMediaProperties
                    {
                        Text = "Thunder",
                        Emoji = new Emoji("⚡")
                    }
                ],
                AllowMultiselect = true,
                LayoutType = PollLayout.Default
            };
            return poll;
        }
        private static async Task<List<List<string>>> ConvertPollChoicesToList(IUserMessage message)
        {
            Logger.LogInformation("Reading data from nade poll");
            List<List<string>> result = new();
            var pollAnswears = message.Poll.Value.Answers;
            foreach (var answerType in pollAnswears)
            {
                var voters = await message.GetPollAnswerVotersAsync(answerType.AnswerId).FlattenAsync();
                List<string> votersList = new();
                foreach (IUser voter in voters)
                {
                    votersList.Add(voter.Username);
                }
                result.Add(votersList);
            }
            return result;
        }
    }
}
