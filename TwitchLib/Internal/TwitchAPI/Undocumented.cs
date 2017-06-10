﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwitchLib.Internal.TwitchAPI
{
    public static class Undocumented
    {
        #region GetClipChat
        public async static Task<Models.API.Undocumented.ClipChat.GetClipChatResponse> GetClipChat(string slug)
        {
            var clip = await v5.GetClip(slug);
            if (clip == null)
                return null;

            string vodId = $"v{clip.VOD.Id}";
            string offsetTime = clip.VOD.Url.Split('=')[1];
            long offsetSeconds = 2; // for some reason, VODs have 2 seconds behind where clips start

            if (offsetTime.Contains("h"))
            {
                offsetSeconds += int.Parse(offsetTime.Split('h')[0]) * 60 * 60;
                offsetTime = offsetTime.Replace(offsetTime.Split('h')[0] + "h", "");
            }
            if (offsetTime.Contains("m"))
            {
                offsetSeconds += int.Parse(offsetTime.Split('m')[0]) * 60;
                offsetTime = offsetTime.Replace(offsetTime.Split('m')[0] + "m", "");
            }
            if (offsetTime.Contains("s"))
                offsetSeconds += int.Parse(offsetTime.Split('s')[0]);

            var rechatResource = $"https://rechat.twitch.tv/rechat-messages?video_id={vodId}&offset_seconds={offsetSeconds}";
            return await Requests.GetGeneric<Models.API.Undocumented.ClipChat.GetClipChatResponse>(rechatResource);
        }
        #endregion
        #region GetTwitchPrimeOffers
        public async static Task<Models.API.Undocumented.TwitchPrimeOffers.TwitchPrimeOffers> GetTwitchPrimeOffers()
        {
            return await Requests.GetGeneric<Models.API.Undocumented.TwitchPrimeOffers.TwitchPrimeOffers>($"https://api.twitch.tv/api/premium/offers?on_site=1");
        }
        #endregion
        #region GetChannelHosts
        public async static Task<Models.API.Undocumented.Hosting.ChannelHostsResponse> GetChannelHosts(string channelId)
        {
            return await Requests.GetSimpleGeneric<Models.API.Undocumented.Hosting.ChannelHostsResponse>($"https://tmi.twitch.tv/hosts?include_logins=1&target={channelId}");
        }
        #endregion
        #region GetChatProperties
        public async static Task<Models.API.Undocumented.ChatProperties.ChatProperties> GetChatProperties(string channelName)
        {
            return await Requests.GetGeneric<Models.API.Undocumented.ChatProperties.ChatProperties>($"https://api.twitch.tv/api/channels/{channelName}/chat_properties");
        }
        #endregion
        #region GetChannelPanels
        public async static Task<Models.API.Undocumented.ChannelPanels.Panel[]> GetChannelPanels(string channelName)
        {
            return await Requests.GetGeneric<Models.API.Undocumented.ChannelPanels.Panel[]>($"https://api.twitch.tv/api/channels/{channelName}/panels");
        }
        #endregion
        #region GetCSMaps
        public async static Task<Models.API.Undocumented.CSMaps.CSMapsResponse> GetCSMaps()
        {
            return await Requests.GetGeneric<Models.API.Undocumented.CSMaps.CSMapsResponse>("https://api.twitch.tv/api/cs/maps");
        }
        #endregion
        #region GetCSStreams
        public async static Task<Models.API.Undocumented.CSStreams.CSStreams> GetCSStreams(int limit = 25, int offset = 0)
        {
            string paramsStr = $"?limit={limit}&offset={offset}";
            return await Requests.GetGeneric<Models.API.Undocumented.CSStreams.CSStreams>($"https://api.twitch.tv/api/cs{paramsStr}"); 
        }
        #endregion
        #region GetRecentMessages
        public async static Task<Models.API.Undocumented.RecentMessages.RecentMessagesResponse> GetRecentMessages(string channelId)
        {
            return await Requests.GetGeneric<Models.API.Undocumented.RecentMessages.RecentMessagesResponse>($"https://tmi.twitch.tv/api/rooms/{channelId}/recent_messages");
        }
        #endregion
        #region GetChatters
        public async static Task<List<Models.API.Undocumented.Chatters.ChatterFormatted>> GetChatters(string channelName)
        {
            var resp = await Requests.GetGeneric<Models.API.Undocumented.Chatters.ChattersResponse>($"https://tmi.twitch.tv/group/user/{channelName}/chatters");

            List<Models.API.Undocumented.Chatters.ChatterFormatted> chatters = new List<Models.API.Undocumented.Chatters.ChatterFormatted>();
            foreach (var chatter in resp.Chatters.Staff)
                chatters.Add(new Models.API.Undocumented.Chatters.ChatterFormatted(chatter, Enums.UserType.Staff));
            foreach (var chatter in resp.Chatters.Admins)
                chatters.Add(new Models.API.Undocumented.Chatters.ChatterFormatted(chatter, Enums.UserType.Admin));
            foreach (var chatter in resp.Chatters.GlobalMods)
                chatters.Add(new Models.API.Undocumented.Chatters.ChatterFormatted(chatter, Enums.UserType.GlobalModerator));
            foreach (var chatter in resp.Chatters.Moderators)
                chatters.Add(new Models.API.Undocumented.Chatters.ChatterFormatted(chatter, Enums.UserType.Moderator));
            foreach (var chatter in resp.Chatters.Viewers)
                chatters.Add(new Models.API.Undocumented.Chatters.ChatterFormatted(chatter, Enums.UserType.Viewer));

            return chatters;
        }
        #endregion
        #region GetRecentChannelEvents
        public async static Task<Models.API.Undocumented.RecentEvents.RecentEvents> GetRecentChannelEvents(string channelId)
        {
            return await Requests.GetGeneric<Models.API.Undocumented.RecentEvents.RecentEvents>($"https://api.twitch.tv/bits/channels/{channelId}/events/recent");
        }
        #endregion

    }
}
