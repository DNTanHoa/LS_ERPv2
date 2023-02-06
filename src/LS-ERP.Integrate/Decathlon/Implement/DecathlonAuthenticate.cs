using LS_ERP.Integrate.Config;
using LS_ERP.Integrate.Decathlon.Interface;
using LS_ERP.Integrate.Decathlon.Model.Authenticate;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LS_ERP.Integrate.Decathlon.Implement
{
    public class DecathlonAuthenticate : IDecathlonAuthenticate
    {
        private readonly DecathlonConfig decathlonConfig;
        private readonly IHttpClientFactory httpClientFactory;

        public DecathlonAuthenticate(IOptions<DecathlonConfig> decathlonConfig,
            IHttpClientFactory httpClientFactory)
        {
            this.decathlonConfig = decathlonConfig.Value;
            this.httpClientFactory = httpClientFactory;
        }

        public string GetAccessToken()
        {
            string result = string.Empty;
            return result;
        }

        #region support function

        public Task<GetDecathlonAccessTokenRespone?> GetRemoteAccessToken(
            GetDecathlonAccessTokenRequest request, string url)
        {
            GetDecathlonAccessTokenRespone? response = null;
            
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("cache-control", "no-cache");
            client.DefaultRequestHeaders.Add("content-type", "application/x-www-form-urlencoded");

            var reqContent = new[]
            {
                new KeyValuePair<string?, string?>("client_id", request.ClientId),
                new KeyValuePair<string?, string?>("client_secret", request.ClientSecret),
                new KeyValuePair<string?, string?>("grant_type", "client_credentials")
            };

            var remoteRespone = client
                .PostAsync(url, new FormUrlEncodedContent(reqContent)).Result;

            if (remoteRespone.IsSuccessStatusCode)
            {
                var remoteResponseContent = remoteRespone.Content.ReadAsStringAsync().Result;
                response = JsonConvert
                    .DeserializeObject<GetDecathlonAccessTokenRespone>(remoteResponseContent);
            }

            return Task.FromResult(response);
        }

        #endregion
    }
}
