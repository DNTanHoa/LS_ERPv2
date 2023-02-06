using LS_ERP.XAF.Module.Service.Request;
using LS_ERP.XAF.Module.Service.Response;
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
    public class PurchaseOrderLineService
    {
        public async Task<MapUPCResponse> MapUPC(MapUPCRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new StringContent(JsonConvert.SerializeObject(request, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                }), Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("MapUPC").ToString(), requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<MapUPCResponse>(responseContent);
                        return responseData;
                    }
                    else
                    {
                        return new MapUPCResponse()
                            .SetResult(false, response.ReasonPhrase);
                    }
                }
                catch (Exception ex)
                {
                    return new MapUPCResponse()
                        .SetResult(false, ex.InnerException.Message);
                }
            }
        }

        public async Task<MapPriceResponse> MapPrice(MapPriceRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new StringContent(JsonConvert.SerializeObject(request,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    }), Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("MapPrice").ToString(), requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<MapPriceResponse>(responseContent);
                        return responseData;
                    }
                    else
                    {
                        return new MapPriceResponse()
                            .SetResult(false, response.ReasonPhrase);
                    }
                }
                catch (Exception ex)
                {
                    return new MapPriceResponse()
                        .SetResult(false, ex.InnerException.Message);
                }
            }
        }

        public async Task<CommonRespone> DeletePurchaseOrderLine(DeletePurchaseOrderLineRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new StringContent(JsonConvert.SerializeObject(request,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    }), Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("DeletePurchaseOrderLine").ToString(),requestContent).Result;
                    
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
