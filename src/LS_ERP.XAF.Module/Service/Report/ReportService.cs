using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service.Report.Response;
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
    public class ReportService
    {
        public async Task<GetItemReportResponse> GetSummaryReport(string keywords,
            string customerID, DateTime fromDate, DateTime toDate)
        {
            var resultResponse = new GetItemReportResponse();

            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");

                try
                {
                    var baseUrl = ConfigurationManager.AppSettings
                        .Get("GetItemReport");
                    var builder = new UriBuilder(ConfigurationManager.AppSettings.Get("Server"));

                    var url = builder + baseUrl + "/" + 
                              fromDate.ToString("yyyy-MM-dd") + "/" +
                              toDate.ToString("yyyy-MM-dd") + "?";

                    if(!string.IsNullOrEmpty(customerID))
                    {
                        url += "customerID=" + customerID;
                    }

                    if (!string.IsNullOrEmpty(keywords))
                    {
                        url += "keywords=?" + keywords;
                    }

                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert
                            .DeserializeObject<GetItemReportResponse>(content);
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

        public async Task<GetSeasonReportResponse> GetSeasonReport(string season,
            string customerID, string style, string keywords)
        {
            var resultResponse = new GetSeasonReportResponse();

            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");

                try
                {
                    var baseUrl = ConfigurationManager.AppSettings
                        .Get("GetSeasonReport");
                    var builder = new UriBuilder(ConfigurationManager.AppSettings.Get("Server"));

                    var url = builder + baseUrl + "?";

                    if (!string.IsNullOrEmpty(customerID))
                    {
                        url += "customerID=" + customerID;
                    }

                    if (!string.IsNullOrEmpty(season))
                    {
                        url += "&season=" + season;
                    }

                    if (!string.IsNullOrEmpty(style))
                    {
                        url += "&style=" + style;
                    }

                    if (!string.IsNullOrEmpty(keywords))
                    {
                        url += "&keywords=" + keywords;
                    }

                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert
                            .DeserializeObject<GetSeasonReportResponse>(content);
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

        public async Task<GetInventoryReportResponse> GetInventoryReportResponse(string customerID,
            string storageCode, string purchaseNumber, string search)
        {
            var resultResponse = new GetInventoryReportResponse();

            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");

                try
                {
                    var baseUrl = ConfigurationManager.AppSettings
                        .Get("GetInventoryReport");
                    var builder = new UriBuilder(ConfigurationManager.AppSettings.Get("Server"));

                    var url = builder + baseUrl + "?";

                    if (!string.IsNullOrEmpty(customerID))
                    {
                        url += "customerID=" + customerID;
                    }

                    if (!string.IsNullOrEmpty(storageCode))
                    {
                        url += "&storageCode=" + storageCode;
                    }

                    if (!string.IsNullOrEmpty(search))
                    {
                        url += "&search=" + search;
                    }

                    if (!string.IsNullOrEmpty(purchaseNumber))
                    {
                        url += "&purchaseNumber=" + purchaseNumber;
                    }

                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        resultResponse = JsonConvert
                            .DeserializeObject<GetInventoryReportResponse>(content);
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
