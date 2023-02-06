using LS_ERP.Mobile.Services.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Mobile.Services.Implement
{
    public class ConfigService
    {
        public string GetHostErpApi()
        {

            return "https://erp-api.sharedataroom.com/api/";
            //return "https://dev-erp-api.sharedataroom.com/api/";            
        }
        public string GetHostHelpDeskApi()
        {
            return "https://api-helpdesk.sharedataroom.com/api/v1/";
            //return "https://dev-api-helpdesk.sharedataroom.com/api/v1/";
        }
    }
}
