// -----------------------------------------------------------------------
// <copyright file="Voting.cs" company="TrickyBestia">
// Copyright (c) TrickyBestia. All rights reserved.
// Licensed under the CC BY-ND 4.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using TrickyBot;

namespace VotingService.API.Features
{
    public static class Voting
    {
        public static Task<VotingInfo> StartVoting(string description, TimeSpan duration, Dictionary<ulong, string> answers, SocketTextChannel channel, IUser author) => Bot.Instance.ServiceManager.GetService<VotingService>().StartVoting(description, duration, answers, channel, author);
    }
}