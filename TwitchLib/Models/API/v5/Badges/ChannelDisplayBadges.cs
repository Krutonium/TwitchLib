﻿namespace TwitchLib.Models.API.v5.Badges
{
    #region using directives
    using Newtonsoft.Json;
    #endregion
    public class ChannelDisplayBadges
    {
        #region BadgeSets
        [JsonProperty(PropertyName = "badge_sets")]
        public BadgeSets Sets { get; protected set; }
        #endregion
    }

    public class BadgeSets
    {
        #region Subscriber
        [JsonProperty(PropertyName = "subscriber")]
        public Badge Subscriber { get; protected set; }
        #endregion

        #region Custom Bit Badges
        [JsonProperty(PropertyName = "bits")]
        public Badge Bits { get; protected set; }
        #endregion
    }
}
