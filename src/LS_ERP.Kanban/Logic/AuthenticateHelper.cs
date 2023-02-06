using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Ultils.Config;
using LS_ERP.Kanban.Logic;
using Common.Model;
using LS_ERP.Kanban.Models;
using Ultils.Extensions;

namespace Ultils.Helpers
{
    public class AuthenticateHelper : IAuthenticateHelper
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly BackEndConfig backEndConfig;

        public AuthenticateHelper(IHttpClientFactory clientFactory,
            IOptions<BackEndConfig> backEndConfig)
        {
            this.clientFactory = clientFactory;
            this.backEndConfig = backEndConfig.Value;
        }

        public string RefreshToken { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public DateTime? ExpiredDate { get; set; }
        public HttpContext? context { get; set; }

        public string GetAccessToken()
        {
            if(ExpiredDate != null &&
               ExpiredDate.Value > DateTime.Now)
            {
                return this.AccessToken;
            }
            else
            {
                var refreshToKenRequest = new
                {
                    RefreshToKen = this.RefreshToken
                };
                var client = this.clientFactory.CreateClient();
                var remoteResponse = client!
                    .ExecutePost<CommonResponseModel<UserLoginDataTransferObject>>
                    (backEndConfig.HostHeplDesk + backEndConfig.RefreshToken, refreshToKenRequest);

                if (remoteResponse.Result.StatusCode ==
                System.Net.HttpStatusCode.OK)
                {
                    var remoteData = remoteResponse.Result.Data;
                    if (remoteData != null)
                    {
                        if (remoteData.Success)
                        {
                            this.AccessToken = remoteData.Data!.AccessToken;
                            this.RefreshToken = remoteData.Data!.RefreshToken;
                            this.ExpiredDate = remoteData.Data!.ExpiredDate;

                            SetResponse(context);
                            return remoteData.Data!.AccessToken ?? string.Empty;
                        }
                    }
                }
            }

            return string.Empty;
        }

        public void SetInforFromContext(HttpContext context)
        {
            this.context = context;
            RefreshToken = context.GetClaimByKey("RefreshToken");
            AccessToken = context.GetClaimByKey("AccessToken");
            ExpiredDate = DateTime.ParseExact(context.GetClaimByKey("ExpiredDate"), "yyyy-MM-dd HH:mm:ss", null);
        }

        public void SetResponse(HttpContext context)
        {
            var identity = context.User.Identity as ClaimsIdentity;
            if(identity != null)
            {
                /// Change refresh token
                var existingRefreshTokenClaim = identity.FindFirst("RefreshToken");
                if (existingRefreshTokenClaim != null)
                    identity.RemoveClaim(existingRefreshTokenClaim);

                identity.AddClaim(new Claim("RefreshToken", this.RefreshToken));

                /// Change access token
                var existingAccessTokenClaim = identity.FindFirst("AccessToken");
                if (existingAccessTokenClaim != null)
                    identity.RemoveClaim(existingAccessTokenClaim);

                identity.AddClaim(new Claim("AccessToken", this.AccessToken));

                /// Change expired date
                var existingExpiredDateClaim = identity.FindFirst("ExpiredDate");
                if (existingExpiredDateClaim != null)
                    identity.RemoveClaim(existingExpiredDateClaim);

                identity.AddClaim(new Claim("ExpiredDate", this.ExpiredDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? String.Empty));
            }
        }
    }
}
