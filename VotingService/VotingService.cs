// -----------------------------------------------------------------------
// <copyright file="VotingService.cs" company="TrickyBestia">
// Copyright (c) TrickyBestia. All rights reserved.
// Licensed under the CC BY-ND 4.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using TrickyBot;
using TrickyBot.API.Abstract;
using TrickyBot.API.Features;
using TrickyBot.Services.SingleServerInfoProviderService;
using VotingService.API.Features;

namespace VotingService
{
    public class VotingService : ServiceBase<VotingServiceConfig>
    {
        private CancellationTokenSource cancellationTokenSource;

        public override ServiceInfo Info { get; } = new ServiceInfo()
        {
            Author = "TrickyBestia",
            Name = "VotingService",
            Version = new Version(1, 0, 0),
            GithubRepositoryUrl = "https://github.com/TrickyBestia/VotingService",
        };

        internal async Task<VotingInfo> StartVoting(string description, TimeSpan duration, Dictionary<ulong, string> answers, SocketTextChannel channel, IUser author)
        {
            var guild = Bot.Instance.ServiceManager.GetService<SingleServerInfoProviderService>().Guild;
            var voting = new VotingInfo()
            {
                Answers = answers,
                Begin = DateTime.Now,
                Description = description,
                End = DateTime.Now + duration,
                BeginMessageChannelId = channel.Id,
            };
            var embedBuilder = new EmbedBuilder();
            var answersText = new StringBuilder();
            foreach (var answer in answers)
            {
                var emote = await guild.GetEmoteAsync(answer.Key);
                answersText.AppendLine($"{emote} - {answer.Value}");
            }

            embedBuilder
                .WithTimestamp(DateTime.Now + duration)
                .WithDescription(description)
                .AddField(this.Config.BeginMessageAnswersTitle, answersText.ToString());

            if (this.Config.ShowAuthor)
            {
                embedBuilder.WithAuthor(author);
            }

            if (!string.IsNullOrEmpty(this.Config.BeginMessageTitle))
            {
                embedBuilder.WithTitle(this.Config.BeginMessageTitle);
            }

            if (!string.IsNullOrEmpty(this.Config.BeginMessageFooter))
            {
                embedBuilder.WithFooter(this.Config.BeginMessageFooter.Replace("{duration}", duration.ToString()));
            }

            var message = await channel.SendMessageAsync(null, false, embedBuilder.Build());
            foreach (var reaction in answers.Keys)
            {
                await message.AddReactionAsync(await guild.GetEmoteAsync(reaction));
            }

            voting.BeginMessageId = message.Id;
            this.Config.ActiveVotings.Add(voting);
            return voting;
        }

        protected override Task OnStart()
        {
            this.cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => this.Timer(this.cancellationTokenSource.Token));
            return Task.CompletedTask;
        }

        protected override Task OnStop()
        {
            this.cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }

        private async Task Timer(CancellationToken cancellationToken)
        {
            var endedVotings = new List<VotingInfo>();
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1d), cancellationToken);
                foreach (var voting in this.Config.ActiveVotings)
                {
                    if (DateTime.Now > voting.End)
                    {
                        endedVotings.Add(voting);
                        await this.SendWinMessageAsync(voting);
                    }
                }

                foreach (var voting in endedVotings)
                {
                    this.Config.ActiveVotings.Remove(voting);
                }

                endedVotings.Clear();
            }
        }

        private async Task SendWinMessageAsync(VotingInfo voting)
        {
            var channel = Bot.Instance.ServiceManager.GetService<SingleServerInfoProviderService>().Guild.GetTextChannel(voting.BeginMessageChannelId);
            var message = await channel.GetMessageAsync(voting.BeginMessageId);
            if (message is null)
            {
                return;
            }

            var winnerEmote = (Emote)message.Reactions.OrderByDescending(reaction =>
            {
                if (reaction.Key is Emote emote)
                {
                    if (voting.Answers.ContainsKey(emote.Id))
                    {
                        return reaction.Value.ReactionCount;
                    }
                }

                return -1;
            }).First().Key;
            var embedBuilder = new EmbedBuilder()
                .WithCurrentTimestamp()
                .WithDescription(voting.Description)
                .AddField(this.Config.EndMessageBeginMessageJumpUrlTitle, message.GetJumpUrl())
                .AddField(this.Config.EndMessageResultTitle, this.Config.EndMessageResultBody.Replace("{winnerEmote}", winnerEmote.ToString()).Replace("{winnerAnswer}", voting.Answers[winnerEmote.Id]));
            if (this.Config.ShowAuthor)
            {
                var messageAuthor = message.Embeds.First().Author;
                var author = new EmbedAuthorBuilder()
                {
                    Name = messageAuthor.Value.Name,
                    Url = messageAuthor.Value.Url,
                    IconUrl = messageAuthor.Value.IconUrl,
                };
                embedBuilder.WithAuthor(author);
            }

            if (!string.IsNullOrEmpty(this.Config.EndMessageFooter))
            {
                embedBuilder.WithFooter(this.Config.EndMessageFooter);
            }

            if (!string.IsNullOrEmpty(this.Config.EndMessageTitle))
            {
                embedBuilder.WithTitle(this.Config.EndMessageTitle);
            }

            await channel.SendMessageAsync(null, false, embedBuilder.Build());
        }
    }
}