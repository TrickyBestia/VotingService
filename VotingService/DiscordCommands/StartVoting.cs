// -----------------------------------------------------------------------
// <copyright file="StartVoting.cs" company="TrickyBestia">
// Copyright (c) TrickyBestia. All rights reserved.
// Licensed under the CC BY-ND 4.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using TrickyBot.Services.DiscordCommandService.API.Abstract;
using TrickyBot.Services.DiscordCommandService.API.Features;
using TrickyBot.Services.DiscordCommandService.API.Features.Conditions;
using VotingService.API.Features;

namespace VotingService.DiscordCommands
{
    public class StartVoting : ConditionDiscordCommand
    {
        public StartVoting()
        {
            this.Conditions.Add(new DiscordCommandPermissionCondition("voting.start"));
        }

        public override string Name { get; } = "voting start";

        public override DiscordCommandRunMode RunMode { get; } = DiscordCommandRunMode.Sync;

        protected override async Task Execute(IMessage message, string parameter)
        {
            var lines = parameter.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var description = lines[0];
            var answers = new Dictionary<ulong, string>();
            for (int i = 1; i < lines.Length - 1; i++)
            {
                var parts = lines[i].Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                answers.Add(Emote.Parse(parts[0]).Id, parts[1]);
            }

            if (answers.Count == 0)
            {
                throw new ArgumentException(nameof(answers));
            }

            char timeModifier = lines[^1][^1];
            double time = double.Parse(lines[^1][..^1]);
            TimeSpan votingDuration = timeModifier switch
            {
                'h' => TimeSpan.FromHours(time),
                'm' => TimeSpan.FromMinutes(time),
                'd' => TimeSpan.FromDays(time),
                _ => throw new ArgumentException($"Invalid time modifier {timeModifier}.")
            };
            await message.DeleteAsync();
            await Voting.StartVoting(description, votingDuration, answers, (SocketTextChannel)message.Channel, message.Author);
        }
    }
}