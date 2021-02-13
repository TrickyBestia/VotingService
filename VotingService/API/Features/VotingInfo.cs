// -----------------------------------------------------------------------
// <copyright file="VotingInfo.cs" company="TrickyBestia">
// Copyright (c) TrickyBestia. All rights reserved.
// Licensed under the CC BY-ND 4.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace VotingService.API.Features
{
    public class VotingInfo
    {
        internal VotingInfo()
        {
        }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public ulong BeginMessageId { get; set; }

        public ulong BeginMessageChannelId { get; set; }

        public string Description { get; set; } = string.Empty;

        public Dictionary<ulong, string> Answers { get; set; } = new Dictionary<ulong, string>();
    }
}