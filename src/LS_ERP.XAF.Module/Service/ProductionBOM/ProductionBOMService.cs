using LS_ERP.XAF.Module.Service.Request;
using LS_ERP.XAF.Module.Service.Response;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class ProductionBOMService
    {
        public async Task<GroupToPurchaseOrderLineRespone> GroupToPurchaseOrderLine(
            GroupToPurchaseOrderLineRequest request)
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
                        .Get("GroupToPurchaseOrderLine").ToString(), requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<GroupToPurchaseOrderLineRespone>(responseContent);
                        return responseData;
                    }
                    else
                    {
                        return new GroupToPurchaseOrderLineRespone().SetResult(false, response.ReasonPhrase);
                    }
                }
                catch (Exception ex)
                {
                    return new GroupToPurchaseOrderLineRespone().SetResult(false, ex.InnerException.Message);
                }
            }
        }
        
        public async Task<CommonRespone> Delete(DeleteProductionBOMRequest request)
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
                        .Get("DeleteProductionBOM").ToString(), requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CommonRespone>(responseContent);
                        return responseData;
                    }
                    else
                    {
                        return new CommonRespone().SetResult(false, response.ReasonPhrase);
                    }
                }
                catch (Exception ex)
                {
                    return new CommonRespone().SetResult(false, ex.InnerException.Message);
                }
            }
        }

        public async Task<GroupToPurchaseRequestLineRespone> GroupToPurchaseRequestLine(
            GroupToPurchaseRequestLineRequest request)
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
                        .Get("GroupToPurchaseRequestLine").ToString(), requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert
                            .DeserializeObject<GroupToPurchaseRequestLineRespone>(responseContent);
                        return responseData;
                    }
                    else
                    {
                        return new GroupToPurchaseRequestLineRespone().SetResult(false, 
                            response.ReasonPhrase);
                    }
                }
                catch (Exception ex)
                {
                    return new GroupToPurchaseRequestLineRespone().SetResult(false, 
                        ex.InnerException.Message);
                }
            }
        }

        public async Task<GroupToIssuedLineResponse> GroupToIssuedLine(
            GroupToIssuedLineRequest request)
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
                        .Get("GroupToIssuedLine").ToString(), requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<GroupToIssuedLineResponse>(responseContent);
                        return responseData;
                    }
                    else
                    {
                        return new GroupToIssuedLineResponse().SetResult(false, response.ReasonPhrase);
                    }
                }
                catch (Exception ex)
                {
                    return new GroupToIssuedLineResponse().SetResult(false, ex.InnerException.Message);
                }
            }
        }
    }
}
