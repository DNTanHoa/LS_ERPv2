using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class ServiceClient
    {
        public ServiceClient()
        {

        }

        public static HttpClient client { get; set; }

        public static HttpClient GetDefaultClient()
        {
            if (client != null)
                return client;
            client = new HttpClient();
            return client;
        }
    }
}
