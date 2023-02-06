using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class PurchaseOrderGroupLineService
    {
        public async Task<CommonRespone> DeletePurchaseOrderGroupLine(long PurchaseOrderGroupLineID)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                try
                {
                    var response = client.DeleteAsync(ConfigurationManager.AppSettings
                        .Get("DeletePurchaseOrderGroupLine").ToString()
                        + PurchaseOrderGroupLineID).Result;
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert
                            .DeserializeObject<CommonRespone>(responseContent);
                        return responseData;
                    }
                    else
                    {
                        return new CommonRespone()
                            .SetResult(false, response.ReasonPhrase);
                    }
                }
                catch(Exception ex)
                {
                    return new CommonRespone()
                        .SetResult(false, ex.InnerException.Message);
                }
            }
        }
    }
}
