using Common.Model;
using DevExpress.ExpressApp.Validation;
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
    public class ReceiptService
    {
        public async Task<CommonRespone> CreateReceipt(CreateReceiptRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);
                var requestContent = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings.Get("CreateReceipt").ToString(),
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
                    return new CommonRespone().SetResult(false, ex.InnerException.Message);
                }
            }

            return null;
        }
        public async Task<CommonRespone> CreateReceiptFabric(CreateReceiptRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);
                var requestContent = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings.Get("CreateReceiptFabric").ToString(),
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
                    return new CommonRespone().SetResult(false, ex.InnerException.Message);
                }
            }

            return null;
        }
        public async Task<ReceiptReportResponse> GetReport(string numbers, string storageCode,
            DateTime fromDate, DateTime toDate)
        {
            var resultResponse = new ReceiptReportResponse();
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                try
                {
                    var baseUrl = ConfigurationManager.AppSettings
                        .Get("GetReceiptReport");
                    var builder = new UriBuilder(ConfigurationManager.AppSettings.Get("Server"));

                    var url = builder + baseUrl + "/" +
                              fromDate.ToString("yyyy-MM-dd") + "/" +
                              toDate.ToString("yyyy-MM-dd");

                    if (!string.IsNullOrEmpty(storageCode))
                        url += "?storageCode=" + storageCode + "&";

                    if (!string.IsNullOrEmpty(numbers))
                        url += "?numbers=" + numbers;

                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert
                            .DeserializeObject<ReceiptReportResponse>(content);
                        resultResponse.IsSuccess = true;
                        resultResponse.Data = data.Data;
                        return resultResponse;
                    }
                    else
                    {
                        resultResponse.ErrorMessage = response.ReasonPhrase;
                    }
                }
                catch (Exception ex)
                {
                    resultResponse.ErrorMessage = ex.Message;
                    return resultResponse;
                }
            }

            return resultResponse;
        }

        public async Task<CommonRespone> DeleteReceipt(string receiptNumber)
        {
            var resultRespone = new CommonRespone();
            
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var request = new { receiptNumber };
                var requestContent = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings.Get("DeleteReceipt").ToString(),
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
                    return new CommonRespone().SetResult(false, ex.InnerException.Message);
                }
            }

            return resultRespone;
        }
    }
}
