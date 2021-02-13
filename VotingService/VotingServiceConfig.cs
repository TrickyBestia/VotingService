// -----------------------------------------------------------------------
// <copyright file="VotingServiceConfig.cs" company="TrickyBestia">
// Copyright (c) TrickyBestia. All rights reserved.
// Licensed under the CC BY-ND 4.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

using TrickyBot.API.Interfaces;
using VotingService.API.Features;

namespace VotingService
{
    public class VotingServiceConfig : IConfig
    {
        public bool IsEnabled { get; set; } = false;

        public bool ShowAuthor { get; set; } = false;

        public string EndMessageResultTitle { get; set; } = "Voting result:";

        public string EndMessageResultBody { get; set; } = "{winnerEmote} - {winnerAnswer}";

        public string EndMessageBeginMessageJumpUrlTitle { get; set; } = "Voting begin message:";

        public string EndMessageTitle { get; set; } = "Voting ended!";

        public string EndMessageFooter { get; set; } = "Voting ended!";

        public string BeginMessageTitle { get; set; } = "Voting!";

        public string BeginMessageFooter { get; set; } = "Ends after {duration}";

        public string BeginMessageAnswersTitle { get; set; } = "Answers:";

        public List<VotingInfo> ActiveVotings { get; set; } = new List<VotingInfo>();
    }
}