namespace TwitchLib.Internal
{
    #region using directives
    using Newtonsoft.Json;
    using System.IO;
    using System.Net;
    using Exceptions.API;
    using Newtonsoft.Json.Serialization;
    using System.Threading.Tasks;
    using System;
    #endregion
    internal class Requests
    {
        public enum API
        {
            v3 = 3,
            v5 = 5,
            Helix = 6,
            Void = 0
        }

        #region POST
        #region PostGenericModel
        public async static Task<T> PostGenericModelAsync<T>(string url, Models.API.RequestModel model, string accessToken = null, API api = API.v5, string clientId = null)
        {
            if (model != null)
                return JsonConvert.DeserializeObject<T>(await generalRequestAsync(url, "POST", TwitchLibJsonSerializer.SerializeObject(model), accessToken, api, clientId), TwitchLibJsonDeserializer);
            else
                return JsonConvert.DeserializeObject<T>(await generalRequestAsync(url, "POST", "", accessToken, api), TwitchLibJsonDeserializer);
        }
        #endregion
        #region PostGeneric
        public async static Task<T> PostGenericAsync<T>(string url, string payload, string accessToken = null, API api = API.v5, string clientId = null)
        {
            return JsonConvert.DeserializeObject<T>(await generalRequestAsync(url, "POST", payload, accessToken, api, clientId), TwitchLibJsonDeserializer);
        }
        #endregion
        #region PostModel
        public async static Task PostModelAsync(string url, Models.API.RequestModel model, string accessToken = null, API api = API.v5, string clientId = null)
        {
            await generalRequestAsync(url, "POST", TwitchLibJsonSerializer.SerializeObject(model), accessToken, api, clientId);
        }
        #endregion
        #region Post
        public async static Task PostAsync(string url, string payload, string accessToken = null, API api = API.v5, string clientId = null)
        {
            await generalRequestAsync(url, "POST", payload, accessToken, api, clientId);
        }
        #endregion
        #endregion
        #region GET
        #region GetGenericAsync
        public async static Task<T> GetGenericAsync<T>(string url, string accessToken = null, API api = API.v5, string clientId = null)
        {
            return JsonConvert.DeserializeObject<T>(await generalRequestAsync(url, "GET", null, accessToken, api, clientId), TwitchLibJsonDeserializer);
        }
        #endregion
        #region GetSimpleGenericAsync
        public async static Task<T> GetSimpleGenericAsync<T>(string url)
        {
            return JsonConvert.DeserializeObject<T>(await simpleRequestAsync(url), TwitchLibJsonDeserializer);
        }
        #endregion
        #endregion
        #region DELETE
        #region Delete
        public async static Task<string> DeleteAsync(string url, string accessToken = null, API api = API.v5, string clientId = null)
        {
            return await generalRequestAsync(url, "DELETE", null, accessToken, api, clientId);
        }
        #endregion
        #region DeleteGenericAsync
        public async static Task<T> DeleteGenericAsync<T>(string url, string accessToken = null, API api = API.v5, string clientId = null)
        {
            return JsonConvert.DeserializeObject<T>(await generalRequestAsync(url, "DELETE", null, accessToken, api, clientId), TwitchLibJsonDeserializer);
        }
        #endregion



        #endregion
        #region PUT
        #region PutGeneric
        public async static Task<T> PutGeneric<T>(string url, string payload, string accessToken = null, API api = API.v5, string clientId = null)
        {
            return JsonConvert.DeserializeObject<T>(await generalRequestAsync(url, "PUT", payload, accessToken, api, clientId), TwitchLibJsonDeserializer);
        }
        #endregion
        #region Put
        public async static Task<string> PutAsync(string url, string payload, string accessToken = null, API api = API.v5, string clientId = null)
        {
            return await generalRequestAsync(url, "PUT", payload, accessToken, api, clientId);
        }
        #endregion
        #region PutBytesAsync
        public static void PutBytes(string url, byte[] payload)
        {
            try
            {
                using (var client = new WebClient())
                    client.UploadData(new Uri(url), "PUT", payload);
            }
            catch (WebException ex) { handleWebException(ex); }
        }
        #endregion
        #endregion

        #region generalRequestAsync
        private async static Task<string> generalRequestAsync(string url, string method, object payload = null, string accessToken = null, API api = API.v5, string clientId = null)
        {
            if (string.IsNullOrEmpty(clientId))
                checkForCredentials(accessToken);
            if(!string.IsNullOrEmpty(clientId) || !string.IsNullOrEmpty(TwitchAPI.Shared.ClientId))
                url = appendClientId(url, clientId);

            var request = WebRequest.CreateHttp(url);
            request.Method = method;
            request.ContentType = "application/json";

            if(api == API.Helix)
                request.Accept = "application/json";
            else  if (api != API.Void)
                request.Accept = $"application/vnd.twitchtv.v{(int)api}+json";

            if (!string.IsNullOrEmpty(accessToken))
                request.Headers["Authorization"] = $"OAuth {Common.Helpers.FormatOAuth(accessToken)}";
            else if (!string.IsNullOrEmpty(TwitchAPI.Shared.AccessToken))
                request.Headers["Authorization"] = $"OAuth {TwitchAPI.Shared.AccessToken}";

            if (payload != null)
                using (var writer = new StreamWriter(await request.GetRequestStreamAsync()))
                    writer.Write(payload);

            try
            {
                var response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string data = reader.ReadToEnd();
                    return data;
                }
            }
            catch (WebException ex) { handleWebException(ex); }

            return null;
        }
        #endregion
        #region simpleRequestAsync
        // credit: https://stackoverflow.com/questions/14290988/populate-and-return-entities-from-downloadstringcompleted-handler-in-windows-pho
        public async static Task<string> simpleRequestAsync(string url)
        {
            var tcs = new TaskCompletionSource<string>();
            var client = new WebClient();

            DownloadStringCompletedEventHandler h = null;
            h = (sender, args) =>
            {
                if (args.Cancelled)
                    tcs.SetCanceled();
                else if (args.Error != null)
                    tcs.SetException(args.Error);
                else
                    tcs.SetResult(args.Result);

                client.DownloadStringCompleted -= h;
            };

            client.DownloadStringCompleted += h;
            client.DownloadString(new Uri(url));

            return await tcs.Task;
        }
        #endregion
        #region requestReturnResponseCode
        public static int RequestReturnResponseCode(string url, string requestType)
        {
            var req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = requestType;
            var response = (HttpWebResponse)req.GetResponse();
            return (int)response.StatusCode;
        }
        #endregion

        #region appendClientId
        private static string appendClientId(string url, string clientId = null)
        {
            if (clientId == null)
                return url.Contains("?")
                    ? $"{url}&client_id={TwitchAPI.Shared.ClientId}"
                    : $"{url}?client_id={TwitchAPI.Shared.ClientId}";
            else
                return url.Contains("?")
                    ? $"{url}&client_id={clientId}"
                    : $"{url}?client_id={clientId}";
        }
        #endregion
        #region checkForCredentials
        private static void checkForCredentials(string passedAccessToken)
        {
            if (string.IsNullOrEmpty(TwitchAPI.Shared.ClientId) && string.IsNullOrWhiteSpace(TwitchAPI.Shared.AccessToken) && string.IsNullOrEmpty(passedAccessToken))
                throw new InvalidCredentialException("All API calls require Client-Id or OAuth token. Set Client-Id by using SetClientId(\"client_id_here\")");
        }
        #endregion

        #region handleWebException
        private static void handleWebException(WebException e)
        {
            HttpWebResponse errorResp = e.Response as HttpWebResponse;
            if (errorResp == null)
                throw e;
            switch (errorResp.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw new BadRequestException("Your request failed because either: \n 1. Your ClientID was invalid/not set.\n 2. You requested a username when the server was expecting a user ID.");
                case HttpStatusCode.Unauthorized:
                    throw new BadScopeException("Your request was blocked due to bad credentials (do you have the right scope for your access token?).");
                case HttpStatusCode.NotFound:
                    throw new BadResourceException("The resource you tried to access was not valid.");
                case (HttpStatusCode)422:
                    throw new NotPartneredException("The resource you requested is only available to channels that have been partnered by Twitch.");
                default:
                    throw e;
            }
        }
        #endregion

        #region SerialiazationSettings
        public static JsonSerializerSettings TwitchLibJsonDeserializer = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };

        public class TwitchLibJsonSerializer
        {
            private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                ContractResolver = new LowercaseContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            public static string SerializeObject(object o)
            {
                return JsonConvert.SerializeObject(o, Formatting.Indented, Settings);
            }

            public class LowercaseContractResolver : DefaultContractResolver
            {
                protected override string ResolvePropertyName(string propertyName)
                {
                    return propertyName.ToLower();
                }
            }
        }
        #endregion
    }
}