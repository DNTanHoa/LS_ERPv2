using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Integrate.Decathlon.Model.Authenticate
{
    public class GetDecathlonAccessTokenRequest
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string GrantType { get; set; } = string.Empty;
    }

    public class GetDecathlonAccessTokenRespone
    {
        public string access_token { get;set; } = string.Empty;
        public string token_type { get; set; } = string.Empty;
        public int expires_in { get; set; }
    }
}
