using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Integrate.Decathlon.Model.Authenticate
{
    public class DecathlonAccessToken
    {
        public static string AccessToken { get; set; } = string.Empty;
        public static DateTime ExpireAt { get; set; }
    }
}
