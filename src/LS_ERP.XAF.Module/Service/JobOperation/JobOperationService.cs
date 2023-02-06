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
    public class JobOperationService
    {
        public async Task<GetJobOperationResponse> GetJobOperations(string customerID,
            DateTime fromDate, DateTime toDate)
        {
            using (var client = new HttpClient())
            {
                var resultResponse = new GetJobOperationResponse();

                try
                {
                    var baseUrl = ConfigurationManager.AppSettings
                        .Get("GetJobOperation");
                    var builder = new UriBuilder(ConfigurationManager.AppSettings.Get("Server"));

                    var url = builder + baseUrl + "?fromDate=" +
                              fromDate.ToString("yyyy-MM-dd") + "&toDate=" +
                              toDate.ToString("yyyy-MM-dd") + "&";

                    if (!string.IsNullOrEmpty(customerID))
                    {
                        url += "customerID=" + customerID;
                    }

                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert
                            .DeserializeObject<GetJobOperationResponse>(content);
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
                    return resultResponse;
                }
            }

            return null;  
        }

        public async Task<BulkJobOperationResponse> BulkJobOperation(BulkJobOperationRequest request)
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
                        .Get("BulkJobOperation").ToString(), requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert
                            .DeserializeObject<BulkJobOperationResponse>(responseContent);
                        return responseData;
                    }
                    else
                    {
                        return new BulkJobOperationResponse()
                        {
                            Success = false,
                            Message = response.ReasonPhrase
                        };
                    }
                }
                catch(Exception ex)
                {
                    return new BulkJobOperationResponse()
                    {
                        Success = false,
                        Message = ex.Message
                    };
                }
            }
        }
    }
}
