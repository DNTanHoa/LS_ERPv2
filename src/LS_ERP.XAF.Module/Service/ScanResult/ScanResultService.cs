using Common.Model;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.BaseImpl.EF;
using LS_ERP.XAF.Module.Service.Report.Response;
using LS_ERP.XAF.Module.Service.Request;
using LS_ERP.XAF.Module.Service.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class ScanResultService
    {
        public async Task<GetScanResultResponse> GetResultSummary(
            GetScanResultSummaryRequest request)
        {
            var result = new GetScanResultResponse();

            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);

                try
                {
                    var baseUrl = ConfigurationManager.AppSettings
                        .Get("GetScanResultSummary");
                    var builder = new UriBuilder(ConfigurationManager.AppSettings.Get("Server"));

                    var url = builder + baseUrl + "?";

                    if (!string.IsNullOrEmpty(request.Company))
                    {
                        url += "company=" + request.Company;
                    }

                    url += "&summaryDate=" + request.SummaryDate.ToString("yyyy-MM-dd");

                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        result = JsonConvert
                            .DeserializeObject<GetScanResultResponse>(content);
                    }
                    else
                    {
                        result.SetResult(false, response.ReasonPhrase);
                    }
                }
                catch (Exception ex)
                {
                    result.SetResult(false, ex.Message);
                }
            }

            return result;
        }

        public async Task<CommonRespone> UpdatePurchaseOrderNumber()
        {
            var result = new CommonRespone();

            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);
                var requestContent = new StringContent(JsonConvert.SerializeObject(new {}),
                    Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PutAsync(ConfigurationManager.AppSettings
                        .Get("UpdateScanResultPurchaseOrderNumber").ToString(),
                       requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CommonRespone>(responseContent);
                        return responseData;
                    }
                }
                catch (Exception ex)
                {
                    result = new CommonRespone().SetResult(false, ex.Message);
                }
            }

            return result;
        }

        public async Task<CommonResponseModel<object>> DeleteScanResults(
            List<long> scanResultIDs, string userName)
        {
            var result = new CommonResponseModel<object>();
            
            using(var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);

                try
                {
                    var requestContent = new StringContent(JsonConvert.SerializeObject(
                        new { data = scanResultIDs, userName = userName }),
                    Encoding.UTF8, "application/json");
                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("DeleteScanResults").ToString(),
                       requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CommonResponseModel<object>>(responseContent);
                        return responseData;
                    }

                }
                catch(Exception ex)
                {
                    result = new CommonResponseModel<object>()
                        .SetResult(false, ex.Message);
                }
            }

            return result;
        }

        public async Task<CommonResponseModel<object>> ConfirmScanResults(List<long> scanResultIDs)
        {
            var result = new CommonResponseModel<object>();

            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);

                try
                {
                    var requestContent = new StringContent(JsonConvert.SerializeObject(
                        new { data = scanResultIDs,}),
                    Encoding.UTF8, "application/json");
                    var response = client.PutAsync(ConfigurationManager.AppSettings
                        .Get("ConfirmScanResults").ToString(),
                       requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CommonResponseModel<object>>(responseContent);
                        return responseData;
                    }
                }
                catch (Exception ex)
                {
                    result = new CommonResponseModel<object>()
                        .SetResult(false, ex.Message);
                }
            }

            return result;
        }
    }
}
