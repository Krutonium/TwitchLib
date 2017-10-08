﻿namespace TwitchLib
{
    #region using directives
    using System.Threading.Tasks;
    using TwitchLib.Api;
    using TwitchLib.Enums;
    #endregion
    public class Blocks
    {
        public Blocks(TwitchAPI api)
        {
            v3 = new V3(api);
        }

        public V3 v3 { get; }

        public class V3 : ApiSection
        {
            public V3(TwitchAPI api) : base(api)
            { }

            #region GetBlocks
            public async Task<Models.API.v3.Blocks.GetBlocksResponse> GetBlocksAsync(string channel, int limit = 25, int offset = 0, string accessToken = null)
            {
                Api.Settings.DynamicScopeValidation(Enums.AuthScopes.User_Blocks_Read, accessToken);
                string pm = $"?limit={limit}&offset={offset}";
                return await Api.GetGenericAsync<Models.API.v3.Blocks.GetBlocksResponse>($"https://api.twitch.tv/kraken/users/{channel}/blocks{pm}", accessToken, ApiVersion.v3).ConfigureAwait(false);
            }
            #endregion
            #region CreateBlock
            public async Task<Models.API.v3.Blocks.Block> CreateBlockAsync(string channel, string target, string accessToken = null)
            {
                Api.Settings.DynamicScopeValidation(Enums.AuthScopes.User_Blocks_Edit, accessToken);
                return await Api.PutGenericAsync<Models.API.v3.Blocks.Block>($"https://api.twitch.tv/kraken/users/{channel}/blocks/{target}", null, accessToken, ApiVersion.v3).ConfigureAwait(false);
            }
            #endregion
            #region RemoveBlock
            public async Task RemoveBlockAsync(string channel, string target, string accessToken = null)
            {
                Api.Settings.DynamicScopeValidation(Enums.AuthScopes.User_Blocks_Edit, accessToken);
                await Api.DeleteAsync($"https://api.twitch.tv/kraken/users/{channel}/blocks/{target}", accessToken, ApiVersion.v3).ConfigureAwait(false);
            }
            #endregion
        }

    }
}
