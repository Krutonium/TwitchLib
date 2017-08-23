﻿namespace TwitchLib.Events.Client
{
    #region using directives
    using System;
    #endregion
    /// <summary>Args representing on channel joined event.</summary>
    public class OnJoinedChannelArgs : EventArgs
    {
        /// <summary>Property representing bot username.</summary>
        public string BotUsername;
        /// <summary>Property representing the channel that was joined.</summary>
        public string Channel;
    }
}
