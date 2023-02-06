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
    public class StorageDetailService
    {
        public async Task<CommonRespone> ImportStorageDetail(ImportStorageDetailRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new MultipartFormDataContent();

                if (request.CustomerID != null)
                    requestContent.Add(new StringContent(request.CustomerID), "CustomerID");

                if (request.StorageCode != null)
                    requestContent.Add(new StringContent(request.StorageCode), "StorageCode");

                if (request.UserName != null)
                    requestContent.Add(new StringContent(request.UserName), "UserName");

                if (File.Exists(request.FilePath))
                {
                    using (var fs = new FileStream(request.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] data;

                        using (var br = new BinaryReader(fs, Encoding.UTF8))
                        {
                            data = br.ReadBytes((int)fs.Length);
                            requestContent.Add(new StreamContent(new MemoryStream(data)), "File",
                                Path.GetFileName(request.FilePath));
                        }

                        fs.Close();
                    }
                }

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("ImportStorageDetail").ToString(),
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

        public async Task<StorageDetailReportResponse> GetReport(string customerID, string storageCode,
            DateTime? fromDate, DateTime? toDate, string productionMethodCode, decimal onHandQuantity)
        {
            var resultResponse = new StorageDetailReportResponse();
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                try
                {
                    client.Timeout = TimeSpan.FromMinutes(10);

                    var baseUrl = ConfigurationManager.AppSettings
                        .Get("ReportStorageDetail");
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

                    if (!string.IsNullOrEmpty(productionMethodCode))
                        url += "&productionMethodCode=" + productionMethodCode;


                    url += "&onHandQuantity=" + onHandQuantity;

                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert
                            .DeserializeObject<StorageDetailReportResponse>(content);
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
