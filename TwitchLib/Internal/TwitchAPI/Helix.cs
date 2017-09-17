﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchLib.Internal.TwitchAPI
{
    public static class Helix
    {
        public static class Users
        {
            public async static Task<Models.API.Helix.Users.GetUsers.GetUsersResponse> GetUsersAsync(List<string> ids = null, List<string> logins = null, string accessToken = null)
            {
                string getParams = "";
                if (ids != null && ids.Count > 0)
                {
                    string idParams = string.Join("&login=", ids);
                    getParams = $"?id={idParams}";
                }
                if (logins != null && logins.Count > 0)
                {
                    string loginParams = string.Join("&id=", logins);
                    if (getParams == "")
                        getParams = $"?login={loginParams}";
                    else
                        getParams += $"&login={loginParams}";
                }
                return await Requests.GetGenericAsync<Models.API.Helix.Users.GetUsers.GetUsersResponse>($"https://api.twitch.tv/helix/users{getParams}", accessToken, Requests.API.Helix);
            }

            public async static Task<Models.API.Helix.Users.GetUsersFollows.GetUsersFollowsResponse> GetUsersFollows(string after = null, string before = null, int first = 20, string fromId = null, string toId = null)
            {
                string getParams = $"?first={first}";
                if (after != null)
                    getParams += $"&after={after}";
                if (before != null)
                    getParams += $"&before={before}";
                if (fromId != null)
                    getParams += $"&from_id={fromId}";
                if (toId != null)
                    getParams += $"&to_id={toId}";

                return await Requests.GetGenericAsync<Models.API.Helix.Users.GetUsersFollows.GetUsersFollowsResponse>($"https://api.twitch.tv/helix/users/follows{getParams}", api: Requests.API.Helix);
            }

            public async static Task PutUsers(string description, string accessToken = null)
            {
                await Requests.PutAsync($"https://api.twitch.tv/helix/users?description={description}", null, accessToken);
            }
        }
    }
}
