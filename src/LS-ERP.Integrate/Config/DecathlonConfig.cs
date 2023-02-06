using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Integrate.Config
{   
    public class DecathlonConfig
    {
        public string ApiUrl { get; set; } = string.Empty;
        public string AuthUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string GrantType { get; set; } = string.Empty;
    }
}
