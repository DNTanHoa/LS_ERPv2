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
    public  class CuttingOutputService
    {
        public async Task<GetItemCuttingOutputReportResponse> GetCuttingOutputReport(string customerID,
           string lsStyle, DateTime fromDate, DateTime toDate)
        {
            var resultResponse = new GetItemCuttingOutputReportResponse();

            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                try
                {
                    var baseUrl = ConfigurationManager.AppSettings
                        .Get("GetCuttingOutputReport");
                    var builder = new UriBuilder(ConfigurationManager.AppSettings.Get("Server"));

                    var url = builder + baseUrl + "?";
                    if (!string.IsNullOrEmpty(customerID))
                    {
                        url += "customerID=" + customerID;
                    }
                    if (!string.IsNullOrEmpty(lsStyle))
                    {
                        url += "&lsStyle=" + lsStyle;
                    }
                    url += "&fromDate=" + fromDate.ToString("yyyy-MM-dd");
                    url += "&toDate=" + toDate.ToString("yyyy-MM-dd");

                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert
                            .DeserializeObject<GetItemCuttingOutputReportResponse>(content);
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
            return resultResponse;
        }
    }
}
