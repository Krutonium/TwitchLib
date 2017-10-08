﻿namespace TwitchLib
{
    #region using directives
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TwitchLib.Api;
    using TwitchLib.Enums;
    #endregion
    public class Communities
    {
        public Communities(TwitchAPI api)
        {
            v5 = new V5(api);
        }

        public V5 v5 { get; }

        public class V5 : ApiSection
        {
            public V5(TwitchAPI api) : base(api)
            {
            }
            #region GetCommunityByName
            public async Task<Models.API.v5.Communities.Community> GetCommunityByNameAsync(string communityName)
            {
                if (string.IsNullOrWhiteSpace(communityName)) { throw new Exceptions.API.BadParameterException("The community name is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                return await Api.GetGenericAsync<Models.API.v5.Communities.Community>($"https://api.twitch.tv/kraken/communities?name={communityName}", null, ApiVersion.v5).ConfigureAwait(false);
            }
            #endregion
            #region GetCommunityByID
            public async Task<Models.API.v5.Communities.Community> GetCommunityByIDAsync(string communityId)
            {
                if (string.IsNullOrWhiteSpace(communityId)) { throw new Exceptions.API.BadParameterException("The community id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                return await Api.GetGenericAsync<Models.API.v5.Communities.Community>($"https://api.twitch.tv/kraken/communities/{communityId}", null, ApiVersion.v5).ConfigureAwait(false);
            }
            #endregion
            #region UpdateCommunity
            public async Task UpdateCommunityAsync(string communityId, string summary = null, string description = null, string rules = null, string email = null, string authToken = null)
            {
                Api.Settings.DynamicScopeValidation(Enums.AuthScopes.Communities_Edit, authToken);
                if (string.IsNullOrWhiteSpace(communityId)) { throw new Exceptions.API.BadParameterException("The community id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                List<KeyValuePair<string, string>> datas = new List<KeyValuePair<string, string>>();
                if (!string.IsNullOrEmpty(summary))
                    datas.Add(new KeyValuePair<string, string>("status", "\"" + summary + "\""));
                if (!string.IsNullOrEmpty(description))
                    datas.Add(new KeyValuePair<string, string>("description", "\"" + description + "\""));
                if (!string.IsNullOrEmpty(rules))
                    datas.Add(new KeyValuePair<string, string>("rules", "\"" + rules + "\""));
                if (!string.IsNullOrEmpty(email))
                    datas.Add(new KeyValuePair<string, string>("email", "\"" + email + "\""));

                string payload = "";
                if (datas.Count == 0)
                {
                    throw new Exceptions.API.BadParameterException("At least one parameter must be specified: summary, description, rules, email.");
                }
                else if (datas.Count == 1)
                {
                    payload = $"\"{datas[0].Key}\": {datas[0].Value}";
                }
                else
                {
                    for (int i = 0; i < datas.Count; i++)
                    {
                        if ((datas.Count - i) > 1)
                            payload = $"{payload}\"{datas[i].Key}\": {datas[i].Value},";
                        else
                            payload = $"{payload}\"{datas[i].Key}\": {datas[i].Value}";
                    }
                }

                payload = "{" + payload + "}";

                await Api.PutAsync($"https://api.twitch.tv/kraken/communities/{communityId}", payload, authToken, ApiVersion.v5).ConfigureAwait(false);
            }
            #endregion
            #region GetTopCommunities
            public async Task<Models.API.v5.Communities.TopCommunities> GetTopCommunitiesAsync(long? limit = null, string cursor = null)
            {
                List<KeyValuePair<string, string>> datas = new List<KeyValuePair<string, string>>();
                if (limit != null)
                    datas.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
                if (!string.IsNullOrEmpty(cursor))
                    datas.Add(new KeyValuePair<string, string>("cursor", cursor));

                string optionalQuery = string.Empty;
                if (datas.Count > 0)
                {
                    for (int i = 0; i < datas.Count; i++)
                    {
                        if (i == 0) { optionalQuery = $"?{datas[i].Key}={datas[i].Value}"; }
                        else { optionalQuery += $"&{datas[i].Key}={datas[i].Value}"; }
                    }
                }
                return await Api.GetGenericAsync<Models.API.v5.Communities.TopCommunities>($"https://api.twitch.tv/kraken/communities/top{optionalQuery}", null, ApiVersion.v5).ConfigureAwait(false);
            }
            #endregion
            #region GetCommunityBannedUsers
            public async Task<Models.API.v5.Communities.BannedUsers> GetCommunityBannedUsersAsync(string communityId, long? limit = null, string cursor = null, string authToken = null)
            {
                Api.Settings.DynamicScopeValidation(Enums.AuthScopes.Communities_Moderate, authToken);
                if (string.IsNullOrWhiteSpace(communityId)) { throw new Exceptions.API.BadParameterException("The community id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                List<KeyValuePair<string, string>> datas = new List<KeyValuePair<string, string>>();
                if (limit != null)
                    datas.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
                if (!string.IsNullOrEmpty(cursor))
                    datas.Add(new KeyValuePair<string, string>("cursor", cursor));

                string optionalQuery = string.Empty;
                if (datas.Count > 0)
                {
                    for (int i = 0; i < datas.Count; i++)
                    {
                        if (i == 0) { optionalQuery = $"?{datas[i].Key}={datas[i].Value}"; }
                        else { optionalQuery += $"&{datas[i].Key}={datas[i].Value}"; }
                    }
                }
                return await Api.GetGenericAsync<Models.API.v5.Communities.BannedUsers>($"https://api.twitch.tv/kraken/communities/{communityId}/bans{optionalQuery}", authToken, ApiVersion.v5).ConfigureAwait(false);
            }
            #endregion
            #region BanCommunityUser
            public async Task BanCommunityUserAsync(string communityId, string userId, string authToken = null)
            {
                Api.Settings.DynamicScopeValidation(Enums.AuthScopes.Communities_Moderate, authToken);
                if (string.IsNullOrWhiteSpace(communityId)) { throw new Exceptions.API.BadParameterException("The community id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                if (string.IsNullOrWhiteSpace(userId)) { throw new Exceptions.API.BadParameterException("The user id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                await Api.PutAsync($"https://api.twitch.tv/kraken/communities/{communityId}/bans/{userId}", null, authToken, ApiVersion.v5).ConfigureAwait(false);
            }
            #endregion
            #region UnBanCommunityUser
            public async Task UnBanCommunityUserAsync(string communityId, string userId, string authToken = null)
            {
                Api.Settings.DynamicScopeValidation(Enums.AuthScopes.Communities_Moderate, authToken);
                if (string.IsNullOrWhiteSpace(communityId)) { throw new Exceptions.API.BadParameterException("The community id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                if (string.IsNullOrWhiteSpace(userId)) { throw new Exceptions.API.BadParameterException("The user id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                await Api.DeleteAsync($"https://api.twitch.tv/kraken/communities/{communityId}/bans/{userId}", authToken, ApiVersion.v5).ConfigureAwait(false);
            }
            #endregion
            #region CreateCommunityAvatarImage
            public async Task CreateCommunityAvatarImageAsync(string communityId, string avatarImage, string authToken = null)
            {
                Api.Settings.DynamicScopeValidation(Enums.AuthScopes.Communities_Edit, authToken);
                if (string.IsNullOrWhiteSpace(communityId)) { throw new Exceptions.API.BadParameterException("The community id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                if (string.IsNullOrWhiteSpace(avatarImage)) { throw new Exceptions.API.BadParameterException("The avatar image is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                await Api.PostAsync($"https://api.twitch.tv/kraken/communities/{communityId}/images/avatar", "{\"avatar_image\": \"" + @avatarImage + "\"}", authToken);
            }
            #endregion
            #region DeleteCommunityAvatarImage
            public async Task DeleteCommunityAvatarImageAsync(string communityId, string authToken = null)
            {
                Api.Settings.DynamicScopeValidation(Enums.AuthScopes.Communities_Edit, authToken);
                if (string.IsNullOrWhiteSpace(communityId)) { throw new Exceptions.API.BadParameterException("The community id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                await Api.DeleteAsync($"https://api.twitch.tv/kraken/communities/{communityId}/images/avatar", authToken, ApiVersion.v5).ConfigureAwait(false);
            }
            #endregion
            #region CreateCommunityCoverImage
            public async Task CreateCommunityCoverImageAsync(string communityId, string coverImage, string authToken = null)
            {
                Api.Settings.DynamicScopeValidation(Enums.AuthScopes.Communities_Edit, authToken);
                if (string.IsNullOrWhiteSpace(communityId)) { throw new Exceptions.API.BadParameterException("The community id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                if (string.IsNullOrWhiteSpace(coverImage)) { throw new Exceptions.API.BadParameterException("The cover image is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                await Api.PostAsync($"https://api.twitch.tv/kraken/communities/{communityId}/images/cover", "{\"cover_image\": \"" + @coverImage + "\"}", authToken, ApiVersion.v5).ConfigureAwait(false);
            }
            #endregion
            #region DeleteCommunityCoverImage
            public async Task DeleteCommunityCoverImageAsync(string communityId, string authToken = null)
            {
                Api.Settings.DynamicScopeValidation(Enums.AuthScopes.Communities_Edit, authToken);
                if (string.IsNullOrWhiteSpace(communityId)) { throw new Exceptions.API.BadParameterException("The community id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                await Api.DeleteAsync($"https://api.twitch.tv/kraken/communities/{communityId}/images/cover", authToken, ApiVersion.v5).ConfigureAwait(false);
            }
            #endregion
            #region GetCommunityModerators
            public async Task<Models.API.v5.Communities.Moderators> GetCommunityModeratorsAsync(string communityId, string authToken)
            {
                Api.Settings.DynamicScopeValidation(Enums.AuthScopes.Communities_Edit, authToken);
                if (string.IsNullOrWhiteSpace(communityId)) { throw new Exceptions.API.BadParameterException("The community id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                return await Api.GetGenericAsync<Models.API.v5.Communities.Moderators>($"https://api.twitch.tv/kraken/communities/{communityId}/moderators", authToken, ApiVersion.v5).ConfigureAwait(false);
            }
            #endregion
            #region AddCommunityModerator
            public async Task AddCommunityModeratorAsync(string communityId, string userId, string authToken = null)
            {
                Api.Settings.DynamicScopeValidation(Enums.AuthScopes.Communities_Edit, authToken);
                if (string.IsNullOrWhiteSpace(communityId)) { throw new Exceptions.API.BadParameterException("The community id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                if (string.IsNullOrWhiteSpace(userId)) { throw new Exceptions.API.BadParameterException("The user id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                await Api.PutAsync($"https://api.twitch.tv/kraken/communities/{communityId}/moderators/{userId}", null, authToken, ApiVersion.v5).ConfigureAwait(false);
            }
            #endregion
            #region DeleteCommunityModerator
            public async Task DeleteCommunityModeratorAsync(string communityId, string userId, string authToken = null)
            {
                Api.Settings.DynamicScopeValidation(Enums.AuthScopes.Communities_Edit, authToken);
                if (string.IsNullOrWhiteSpace(communityId)) { throw new Exceptions.API.BadParameterException("The community id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                if (string.IsNullOrWhiteSpace(userId)) { throw new Exceptions.API.BadParameterException("The user id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                await Api.DeleteAsync($"https://api.twitch.tv/kraken/communities/{communityId}/moderators/{userId}", authToken, ApiVersion.v5).ConfigureAwait(false);
            }
            #endregion
            #region GetCommunityPermissions
            public async Task<Dictionary<string, bool>> GetCommunityPermissionsAsync(string communityId, string authToken = null)
            {
                Api.Settings.DynamicScopeValidation(Enums.AuthScopes.Any, authToken);
                if (string.IsNullOrWhiteSpace(communityId)) { throw new Exceptions.API.BadParameterException("The community id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                return await Api.GetGenericAsync<Dictionary<string, bool>>($"https://api.twitch.tv/kraken/communities/{communityId}/permissions", authToken, ApiVersion.v5).ConfigureAwait(false);
            }
            #endregion
            #region ReportCommunityViolation
            public async Task ReportCommunityViolationAsync(string communityId, string channelId)
            {
                if (string.IsNullOrWhiteSpace(communityId)) { throw new Exceptions.API.BadParameterException("The community id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                if (string.IsNullOrWhiteSpace(channelId)) { throw new Exceptions.API.BadParameterException("The channel id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                await Api.PostAsync($"https://api.twitch.tv/kraken/communities/{communityId}/report_channel", "{\"channel_id\": \"" + channelId + "\"}", null, ApiVersion.v5).ConfigureAwait(false);
            }
            #endregion
            #region GetCommunityTimedOutUsers
            public async Task<Models.API.v5.Communities.TimedOutUsers> GetCommunityTimedOutUsersAsync(string communityId, long? limit = null, string cursor = null, string authToken = null)
            {
                Api.Settings.DynamicScopeValidation(Enums.AuthScopes.Communities_Moderate, authToken);
                if (string.IsNullOrWhiteSpace(communityId)) { throw new Exceptions.API.BadParameterException("The community id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                List<KeyValuePair<string, string>> datas = new List<KeyValuePair<string, string>>();
                if (limit != null)
                    datas.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
                if (!string.IsNullOrEmpty(cursor))
                    datas.Add(new KeyValuePair<string, string>("cursor", cursor));

                string optionalQuery = string.Empty;
                if (datas.Count > 0)
                {
                    for (int i = 0; i < datas.Count; i++)
                    {
                        if (i == 0) { optionalQuery = $"?{datas[i].Key}={datas[i].Value}"; }
                        else { optionalQuery += $"&{datas[i].Key}={datas[i].Value}"; }
                    }
                }
                return await Api.GetGenericAsync<Models.API.v5.Communities.TimedOutUsers>($"https://api.twitch.tv/kraken/communities/{communityId}/timeouts{optionalQuery}", authToken, ApiVersion.v5).ConfigureAwait(false);
            }
            #endregion
            #region AddCommunityTimedOutUser
            public async Task AddCommunityTimedOutUserAsync(string communityId, string userId, int duration, string reason = null, string authToken = null)
            {
                Api.Settings.DynamicScopeValidation(Enums.AuthScopes.Communities_Moderate, authToken);
                if (string.IsNullOrWhiteSpace(communityId)) { throw new Exceptions.API.BadParameterException("The community id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                if (string.IsNullOrWhiteSpace(userId)) { throw new Exceptions.API.BadParameterException("The user id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                string payload = "{\"duration\": \"" + duration + "\"" + ((!string.IsNullOrWhiteSpace(reason)) ? ", \"reason\": \"" + reason + "\"}" : "}");
                await Api.PutAsync($"https://api.twitch.tv/kraken/communities/{communityId}/timeouts/{userId}", payload, authToken, ApiVersion.v5).ConfigureAwait(false);
            }
            #endregion
            #region DeleteCommunityTimedOutUser
            public async Task DeleteCommunityTimedOutUserAsync(string communityId, string userId, string authToken = null)
            {
                Api.Settings.DynamicScopeValidation(Enums.AuthScopes.Communities_Moderate, authToken);
                if (string.IsNullOrWhiteSpace(communityId)) { throw new Exceptions.API.BadParameterException("The community id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                if (string.IsNullOrWhiteSpace(userId)) { throw new Exceptions.API.BadParameterException("The user id is not valid. It is not allowed to be null, empty or filled with whitespaces."); }
                await Api.DeleteAsync($"https://api.twitch.tv/kraken/communities/{communityId}/timeouts/{userId}", authToken, ApiVersion.v5).ConfigureAwait(false);
            }
            #endregion
        }
    }
}
