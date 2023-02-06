using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class PurchaseOrderService
    {
        public Task<GetPurchaseOrderReportResponse> GetPurchaseOrderReport(string customerID, string vendorID, DateTime fromDate, DateTime toDate)
        {
            var resultResponse = new GetPurchaseOrderReportResponse();
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                try
                {
                    var baseUrl = ConfigurationManager.AppSettings
                        .Get("PurchaseOrderReport");
                    var builder = new UriBuilder(ConfigurationManager.AppSettings.Get("Server"));

                    var url = builder + baseUrl + "?";
                    if (!string.IsNullOrEmpty(customerID))
                    {
                        url += "customerID=" + customerID;
                    }
                    if (!string.IsNullOrEmpty(vendorID))
                    {
                        url += "&vendorID=" + vendorID;
                    }
                    url += "&fromDate=" + fromDate.ToString("yyyy-MM-dd");
                    url += "&toDate=" + toDate.ToString("yyyy-MM-dd");
                    var response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert
                            .DeserializeObject<GetPurchaseOrderReportResponse>(content);
                        resultResponse.Data = data.Data;
                        resultResponse.IsSuccess = true;
                    }
                    else
                    {
                        resultResponse.ErrorMessage = response.ReasonPhrase;
                    }
                }
                catch (Exception ex)
                {
                    resultResponse.ErrorMessage = ex.Message;
                    return Task.FromResult(resultResponse);
                }
            }
            return Task.FromResult(resultResponse);
        }
        public async Task<CommonRespone> CreatePurchaseOrder(CreatePurchaseOrderRequest request)
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
                        .Get("CreatePurchaseOrder").ToString(), requestContent).Result;

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

        public async Task<CommonRespone> UpdatePurchaseOrder(UpdatePurchaseOrderRequest request)
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
                    var response = client.PutAsync(ConfigurationManager.AppSettings
                        .Get("UpdatePurchaseOrder").ToString(), requestContent).Result;

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

        public async Task<CommonRespone> DeletePurchaseOrder(DeletePurchaseOrderRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                try
                {
                    var response = client.DeleteAsync(ConfigurationManager.AppSettings
                        .Get("DeletePurchaseOrder").ToString()
                        + request.PurchaseOrderID
                        + "/" + request.Username).Result;

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
                catch (Exception ex)
                {
                    return new CommonRespone()
                        .SetResult(false, ex.InnerException.Message);
                }
            }
        }

        public async Task<CommonRespone> ImportPurchaseOrder(
            ImportPurchaseOrderRequest request)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(120);
                ConfigurationManager.AppSettings.Get("Server");

                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new MultipartFormDataContent();

                if (request.CustomerID != null)
                    requestContent.Add(new StringContent(request.CustomerID), "CustomerID");

                if (request.UserName != null)
                    requestContent.Add(new StringContent(request.UserName), "UserName");

                if (request.Type != null)
                    requestContent.Add(new StringContent(request.Type.ToString()), "Type");

                if (File.Exists(request.FilePath))
                {
                    using (var fs = new FileStream(request.FilePath, FileMode.Open, FileAccess.Read))
                    {

                        byte[] data;

                        using (var br = new BinaryReader(fs, Encoding.UTF8))
                        {

                            data = br.ReadBytes((int)fs.Length);

                            //ByteArrayContent bytes = new ByteArrayContent(data);
                            requestContent.Add(new StreamContent(new MemoryStream(data)), "File",
                                Path.GetFileName(request.FilePath));
                        }

                        fs.Close();
                    }
                }

                try
                {

                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("ImportPurchaseOrder").ToString(),
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

        public async Task<CommonRespone> MatchingShipmentPurchaseOrder(
            MatchingShipmentPurchaseOrderRequest request)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(120);
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
                        .Get("MatchingShipment").ToString(),
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

        public async Task<PurchaseReceivedResponse> GetReceived(string customerID, string storageCode,
            DateTime? fromDate, DateTime? toDate)
        {
            var resultResponse = new PurchaseReceivedResponse();
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                try
                {
                    client.Timeout = TimeSpan.FromMinutes(15);

                    var baseUrl = ConfigurationManager.AppSettings
                        .Get("PurchaseReceived");
                    var builder = new UriBuilder(ConfigurationManager.AppSettings.Get("Server"));

                    var url = builder + baseUrl + "?";

                    if (fromDate != null)
                    {
                        url += "&fromDate=" + fromDate?.ToString("yyyy-MM-dd");
                    }

                    if (toDate != null)
                    {
                        url += "&toDate=" + toDate?.ToString("yyyy-MM-dd");
                    }

                    if (!string.IsNullOrEmpty(storageCode))
                        url += "&storageCode=" + storageCode;

                    if (!string.IsNullOrEmpty(customerID))
                        url += "&customerID=" + customerID;

                    //if (!string.IsNullOrEmpty(productionMethodCode))
                    //    url += "&productionMethodCode=" + productionMethodCode;

                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert
                            .DeserializeObject<PurchaseReceivedResponse>(content);
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
    }
}
