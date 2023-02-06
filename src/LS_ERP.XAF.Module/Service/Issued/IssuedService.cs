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
    public class IssuedService
    {
        public async Task<CommonRespone> CreateIssued(CreateIssuedRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);
                var requestContent = new StringContent(JsonConvert.SerializeObject(request, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                }), Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("CreateIssued").ToString(), requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CommonRespone>(responseContent);
                        return responseData;
                    }
                    else
                    {
                        return new CommonRespone()
                            .SetResult(false, response.ReasonPhrase);
                    }
                }
                catch (Exception ex)
                {
                    return new CommonRespone()
                        .SetResult(false, ex.InnerException.Message);
                }
            }
        }

        public async Task<CommonRespone> CreateIssuedFabric(CreateIssuedRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);
                var requestContent = new StringContent(JsonConvert.SerializeObject(request, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                }), Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("CreateIssuedFabric").ToString(), requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CommonRespone>(responseContent);
                        return responseData;
                    }
                    else
                    {
                        return new CommonRespone()
                            .SetResult(false, response.ReasonPhrase);
                    }
                }
                catch (Exception ex)
                {
                    return new CommonRespone()
                        .SetResult(false, ex.InnerException.Message);
                }
            }
        }

        public async Task<CommonRespone> UpdateIssued(UpdateIssuedRequest request)
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
                    var response = client.PutAsync(ConfigurationManager.AppSettings
                        .Get("UpdateIssued").ToString(), requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CommonRespone>(responseContent);
                        return responseData;
                    }
                    else
                    {
                        return new CommonRespone()
                            .SetResult(false, response.ReasonPhrase);
                    }
                }
                catch (Exception ex)
                {
                    return new CommonRespone()
                        .SetResult(false, ex.InnerException.Message);
                }
            }
        }

        public async Task<GetIssuedSupplierResponse> GetIssuedSupplier(GetIssuedSupplierRequest request)
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
                        .Get("GetIssuedSupplier").ToString(), requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<GetIssuedSupplierResponse>(responseContent);
                        return responseData;
                    }
                    else
                    {
                        return new GetIssuedSupplierResponse()
                            .SetResult(false, response.ReasonPhrase);
                    }
                }
                catch (Exception ex)
                {
                    return new GetIssuedSupplierResponse()
                        .SetResult(false, ex.InnerException.Message);
                }
            }
        }

        public async Task<GetIssuedReportResponse> GetReport(string customerID, string storageCode,
            DateTime fromDate, DateTime toDate)
        {
            var resultResponse = new GetIssuedReportResponse();
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                try
                {
                    var baseUrl = ConfigurationManager.AppSettings
                        .Get("GetIssuedReport");
                    var builder = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                    var url = builder + baseUrl + "?";

                    url += "fromDate=" + fromDate.ToString("yyyy-MM-dd") + "&";
                    url += "toDate=" + toDate.ToString("yyyy-MM-dd") + "&";

                    if (!string.IsNullOrEmpty(storageCode))
                        url += "&storageCode=" + storageCode;

                    if (!string.IsNullOrEmpty(customerID))
                        url += "&customerID=" + customerID;


                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert
                            .DeserializeObject<GetIssuedReportResponse>(content);
                        resultResponse.SetResult(true, string.Empty);
                        resultResponse.Data = data.Data;
                        return resultResponse;
                    }
                    else
                    {
                        resultResponse.SetResult(false, response.ReasonPhrase);
                    }
                }
                catch (Exception ex)
                {
                    resultResponse.SetResult(false, ex.Message);
                    return resultResponse;
                }
            }

            return resultResponse;
        }
    }
}
