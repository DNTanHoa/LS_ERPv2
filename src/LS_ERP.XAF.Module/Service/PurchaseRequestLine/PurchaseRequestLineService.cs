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
    public class PurchaseRequestLineService
    {
        public async Task<GroupPurchaseRequestLineToPurchaseOrderLineRespone> 
            GroupToPurchaseOrderLine(GroupPurchaseRequestLineToPurchaseOrderLineRequest request)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    ConfigurationManager.AppSettings.Get("Server");
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings
                        .Get("Server").ToString());

                    var requestContent = new StringContent(JsonConvert.SerializeObject(request, 
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        }), Encoding.UTF8, "application/json");

                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("GroupPurchaseRequestToPurchaseOrderLine").ToString(), requestContent)
                        .Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert
                            .DeserializeObject<GroupPurchaseRequestLineToPurchaseOrderLineRespone>(responseContent);
                        return responseData;
                    }
                    else
                    {
                        return new GroupPurchaseRequestLineToPurchaseOrderLineRespone()
                            .SetResult(false, response.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                return new GroupPurchaseRequestLineToPurchaseOrderLineRespone()
                    .SetResult(false, ex.InnerException.Message);
            }
        }
    }
}
